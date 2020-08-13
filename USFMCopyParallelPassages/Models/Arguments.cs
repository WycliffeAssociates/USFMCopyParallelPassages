using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace USFMCopyParallelPassages.Models
{
    public class Arguments
    {
        [Option("usfm",Required = true, HelpText = "Directory for the source USFM")]
        public string USFMSourceDir { get; set; }

        [Option("output",Required = true, HelpText = "Output directory for the generated USFM")]
        public string OutputDir { get; set; }

        [Option("mapping", Required = true, HelpText = "Mapping file")]
        public string MappingFile { get; set; }
    }
}
