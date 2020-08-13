using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using USFMCopyParallelPassages.Models;

namespace USFMCopyParallelPassages
{
    public class CSVMappingLoader : IMappingLoader
    {
        // Note that this excludes a and b and just includes the whole thing
        Regex regex = new Regex("(\\w+) (\\d+):(\\d+)[a|b]?-*(\\d*)[a|b]?");
        public List<(MappingBound source, MappingBound destination)> LoadMapping(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Mapping file {fileName} is not found");
            }

            // Load file contents from CSV

            using var stream = File.OpenRead(fileName);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            var records = csv.GetRecords<CSVMappingModel>();

            // Parse out the CSV
            var output = new List<(MappingBound source, MappingBound destination)>();

            foreach(var item in records)
            {
                var destination = ParseMappingBound(item.Destination);
                var source = ParseMappingBound(item.Source);
                if (destination == null || source == null)
                {
                    continue;
                }

                output.Add((source, destination));
            }

            return output;
        }

        private MappingBound ParseMappingBound(string input)
        {
            var match = regex.Match(input);

            if (!match.Success)
            {
                return null;
            }

            MappingBound output = new MappingBound()
            {
                Book = match.Groups[1].Value.ToLower(),
                Chapter = int.Parse(match.Groups[2].Value),
                StartingVerse = int.Parse(match.Groups[3].Value)
            };
            if (match.Groups.Count > 4 && !string.IsNullOrEmpty(match.Groups[4].Value))
            {
                output.EndingVerse = int.Parse(match.Groups[4].Value);
            }

            return output;
        }
    }
}
