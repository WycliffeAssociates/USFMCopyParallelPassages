using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USFMToolsSharp.Models.Markers;

namespace USFMCopyParallelPassages
{
    public static class USFMDocumentExtensions
    {
        public static List<Marker> GetPassage(this USFMDocument document, int chapterNumber, int startingVerse, int endingVerse)
        {
            var chapter = document.GetChapter(chapterNumber);
            if (chapter == null)
            {
                return null;
            }
            var verses = chapter.GetChildMarkers<VMarker>();

            if(endingVerse != 0)
            {

                verses = verses.Where(v => v.StartingVerse >= startingVerse && ((v.EndingVerse != 0 ? v.EndingVerse : v.StartingVerse) <= endingVerse)).ToList();
            }
            else
            {
                //TODO: come back here and take care of verse bridges in source
                verses = verses.Where(v => v.StartingVerse == startingVerse).ToList();
            }

            verses = verses.OrderBy(v => v.StartingVerse).ToList();

            List<Marker> output = new List<Marker>();
            foreach(var verse in verses)
            {
                output.AddRange(verse.Contents);
            }

            return output;
        }

        public static CMarker GetChapter(this USFMDocument document, int chapter)
        {
            var chapters = document.GetChildMarkers<CMarker>();
            return chapters.FirstOrDefault(c => c.Number == chapter);
        }
    }
}
