using System;
using System.Collections.Generic;
using System.Text;

namespace USFMCopyParallelPassages.Models
{
    public class MappingBound
    {
        public string Book { get; set; }
        public int Chapter { get; set; }
        public int StartingVerse { get; set; }
        public int EndingVerse { get; set; }
    }
}
