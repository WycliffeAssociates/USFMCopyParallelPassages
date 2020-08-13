using System;
using System.Collections.Generic;
using System.Text;
using USFMCopyParallelPassages.Models;

namespace USFMCopyParallelPassages
{
    interface IMappingLoader
    {
        List<(MappingBound source, MappingBound destination)> LoadMapping(string fileName);
    }
}
