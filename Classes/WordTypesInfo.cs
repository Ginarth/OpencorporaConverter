using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpencorporaConverter.Classes
{
    internal class WordTypesInfo
    {
        public int NobodyCount { get; set; }
        public int PredecessorCount { get; set; }
        public int IncumbentCount { get; set; }
        public int SuccessorCount { get; set; }
        public int TotalCount { get; set; }
        public WordTypesInfo(int nobodyCount, int predecessorCount, int incumbentCount,
            int successorCount, int totalCount)
        {
            NobodyCount = nobodyCount;
            PredecessorCount = predecessorCount;
            IncumbentCount = incumbentCount;
            SuccessorCount = successorCount;
            TotalCount = totalCount;
        }

        public WordTypesInfo() { }
    }
}
