using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using USFMCopyParallelPassages.Models;
using USFMToolsSharp;
using USFMToolsSharp.Models.Markers;
using USFMToolsSharp.Renderers.USFM;

namespace USFMCopyParallelPassages
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Arguments>(args).WithParsed((arguments) =>
            {
                var books = new Dictionary<string, USFMDocument>();
                var mapping = LoadMapping(arguments.MappingFile).OrderBy(m => m.destination.Chapter).ThenBy(m => m.destination.StartingVerse).ToList();
                var source = LoadBooks(arguments.USFMSourceDir);
                foreach(var i in mapping)
                {
                    if (!source.ContainsKey(i.source.Book))
                    {
                        Console.WriteLine($"{i.source.Book} doesn't exist in the current source");
                    }
                    if (!books.ContainsKey(i.destination.Book))
                    {
                        books.Add(i.destination.Book, BuildUSFMDocument(i.destination.Book));
                    }

                    // Get content to insert

                    var contentToInsert = source[i.source.Book].GetPassage(i.source.Chapter, i.source.StartingVerse, i.source.EndingVerse);

                    // Ensure chapter in book

                    var chapter = books[i.destination.Book].GetChapter(i.destination.Chapter);
                    if (chapter == null)
                    {
                        chapter = new CMarker() { Number = i.destination.Chapter };
                        books[i.destination.Book].Insert(chapter);
                    }

                    // Ensure verse is there
                    var verse = new VMarker() { VerseNumber = i.destination.EndingVerse == 0 ? i.destination.StartingVerse.ToString() : $"{i.destination.StartingVerse}-{i.destination.EndingVerse}" };

                    chapter.Contents.Add(verse);

                    verse.Contents.AddRange(contentToInsert);
                }

                // Write out book
                foreach(var book in books)
                {
                    WriteUSFM(book.Value, Path.Join(arguments.OutputDir, $"{book.Key}.usfm"));
                }
            });
        }
        private static List<(MappingBound source, MappingBound destination)> LoadMapping(string mappingFile)
        {
            IMappingLoader loader = new CSVMappingLoader();
            return loader.LoadMapping(mappingFile);
        }

        private static Dictionary<string,USFMDocument> LoadBooks(string directory)
        {
            Dictionary<string, USFMDocument> output = new Dictionary<string, USFMDocument>();
            USFMParser parser = new USFMParser(new List<string> { "s5" });
            foreach(var file in Directory.GetFiles(directory, "*.usfm", SearchOption.AllDirectories))
            {
                var tmp = parser.ParseFromString(File.ReadAllText(file));
                var toc3s = tmp.GetChildMarkers<TOC3Marker>();
                if (toc3s.Count != 0)
                {
                    if (output.ContainsKey(toc3s[0].BookAbbreviation.ToLower()))
                    {
                        continue;
                    }

                    output.Add(toc3s[0].BookAbbreviation.ToLower(), tmp);
                }
            }
            return output;
        }

        private static USFMDocument BuildUSFMDocument(string bookAbbreviation)
        {
            USFMDocument document = new USFMDocument();
            document.Contents.Add(new IDEMarker() { Encoding = "utf-8" });
            document.Contents.Add(new TOC3Marker() { BookAbbreviation = bookAbbreviation });
            return document;
        }

        private static void WriteUSFM(USFMDocument document, string path)
        {
            USFMRenderer renderer = new USFMRenderer();
            File.WriteAllText(path, renderer.Render(document));
        }
    }
}
