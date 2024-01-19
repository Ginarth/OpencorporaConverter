using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpencorporaConverter.Classes
{
    internal class WordsInfo
    {
        public int LemmasCount { get; set; }
        public int UniqLemmasCount { get; set; }
        public int FormsCount { get; set; }
        public int UniqFormsCount { get; set; }

        public WordsInfo(int lemmasCount, int uniqLemmasCount, int formsCount, int uniqFormsCount)
        {
            LemmasCount = lemmasCount;
            UniqLemmasCount = uniqLemmasCount;
            FormsCount = formsCount;
            UniqFormsCount = uniqFormsCount;
        }

        public WordsInfo() { }
    }
}
