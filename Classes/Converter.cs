using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpencorporaConverter.Classes
{
    internal class Converter
    {
        private string _directoryPath;
        private Opencorpora _openCorpora;
        private WordLemmaComparer _wordLemmaComparer;
        private WordIdComparer _wordIdComparer;
        private WordsInfo _wi;
        private WordTypesInfo _wti;

        public Converter(Opencorpora opencorpora, string filePath)
        {
            _directoryPath = Path.GetDirectoryName(filePath)!;
            // deserialized xml dictionary
            _openCorpora = opencorpora;
            // comparers for binarySearch
            _wordLemmaComparer = new WordLemmaComparer();
            _wordIdComparer = new WordIdComparer();
            // metrics
            _wi = new WordsInfo();
            _wti = new WordTypesInfo();
        }

        public void Run()
        {
            ExportToCsv(OptimizeDictionary(_openCorpora));
        }

        private List<Word> OptimizeDictionary(Opencorpora opencorpora)
        {
            List<Word> words = CreateWords();

            words = CreateWordTypes(words);

            if (_wi.LemmasCount != _wti.TotalCount)
            {
                throw new Exception("Words.Lemmas != Words.Total");
            }

            int importedCount = 0;
            words = ImportPredecessorForms(words, ref importedCount);

            if (_wti.IncumbentCount + _wti.SuccessorCount != importedCount)
            {
                throw new Exception("Incumbent + Successor != Imported");
            }

            return GetUniqueWords(words);
        }

        private void ExportToCsv(List<Word> words)
        {
            Console.WriteLine("\nExport To Csv");

            //prepare opencorpora_grammemes.csv
            List<string> opencorpora_grammemes = _openCorpora.Grammemes
                .Where(g => g.Parent.Equals("POST"))
                .Select(g => g.Name).ToList();
            Console.WriteLine($"\n" +
                $"opencorpora_grammeme_id opencorpora_grammeme");
            for (int i = opencorpora_grammemes.Count - 10; i < opencorpora_grammemes.Count; i++)
                Console.WriteLine($"{i + 1} {opencorpora_grammemes[i]}");

            //export opencorpora_grammemes.csv
            Console.WriteLine("\nExporting opencorpora_grammemes.csv file...");
            using (StreamWriter writer = new StreamWriter(
                _directoryPath + @$"\opencorpora_grammemes.csv", false))
            {
                writer.WriteLine("opencorpora_grammeme_id opencorpora_grammeme");
                for (int i = 0; i < opencorpora_grammemes.Count; i++)
                {
                    string entry = $"{i + 1} {opencorpora_grammemes[i]}";
                    if (i != opencorpora_grammemes.Count - 1)
                        entry += "\n";
                    writer.Write(entry);
                }
            }

            //opencorpora_lemmas.csv
            List<int> opencorpora_grammeme_id = new List<int>();
            HashSet<string> lemmas = new HashSet<string>();
            foreach (Word word in words)
            {
                opencorpora_grammeme_id.Add(opencorpora_grammemes.IndexOf(word.Grammeme) + 1);
                lemmas.Add(word.Lemma);
            }
            List<string> opencorpora_lemmas = lemmas.ToList();
            if (words.Count != opencorpora_lemmas.Count)
                throw new Exception("Lemmas aren't unique");
            Console.WriteLine($"\n" +
                $"opencorpora_lemma_id opencorpora_grammeme_id " +
                $"opencorpora_lemma");
            for (int i = opencorpora_lemmas.Count - 10; i < opencorpora_lemmas.Count; i++)
                Console.WriteLine($"{i + 1} {opencorpora_grammeme_id[i]} " +
                    $"{opencorpora_lemmas[i]}");

            //export opencorpora_lemmas.csv
            Console.WriteLine("\nExporting opencorpora_lemmas.csv file...");
            using (StreamWriter writer = new StreamWriter(
                _directoryPath + @$"\opencorpora_lemmas.csv", false))
            {
                writer.WriteLine("opencorpora_lemma_id opencorpora_grammeme_id " +
                    "opencorpora_lemma");
                for (int i = 0; i < opencorpora_lemmas.Count; i++)
                {
                    string entry = $"{i + 1} {opencorpora_grammeme_id[i]} " +
                        $"{opencorpora_lemmas[i]}";
                    if (i != opencorpora_lemmas.Count - 1)
                        entry += "\n";
                    writer.Write(entry);
                }
            }

            //opencorpora_forms
            int lemmaId = 1;
            List<int> opencorpora_lemma_id = new List<int>();
            HashSet<string> forms = new HashSet<string>();
            foreach (Word word in words)
            {
                foreach (string form in word.Forms)
                {
                    forms.Add(form);
                    opencorpora_lemma_id.Add(lemmaId);
                }
                lemmaId++;
            }
            List<string> opencorpora_forms = forms.ToList();
            if (forms.Count != opencorpora_forms.Count)
                throw new Exception("Lemmas aren't unique");
            Console.WriteLine($"\n" +
                $"opencorpora_form_id opencorpora_lemma_id " +
                $"opencorpora_form");
            for (int i = opencorpora_forms.Count - 10; i < opencorpora_forms.Count; i++)
                Console.WriteLine($"{i + 1} {opencorpora_lemma_id[i]} " +
                    $"{opencorpora_forms[i]}");

            //export opencorpora_forms.csv
            Console.WriteLine("\nExporting opencorpora_forms.csv file...");
            using (StreamWriter writer = new StreamWriter(
                _directoryPath + @$"\opencorpora_forms.csv", false))
            {
                writer.WriteLine("opencorpora_form_id opencorpora_lemma_id " +
                    "opencorpora_form");
                for (int i = 0; i < opencorpora_forms.Count; i++)
                {
                    string entry = $"{i + 1} {opencorpora_lemma_id[i]} " +
                        $"{opencorpora_forms[i]}";
                    if (i != opencorpora_lemma_id.Count - 1)
                        entry += "\n";
                    writer.Write(entry);
                }
            }

            Console.WriteLine($"\nComplete! Check new files in directory {_directoryPath}");
        }

        private List<Word> GetUniqueWords(List<Word> words)
        {
            List<Word> unique_words = words.Where(w => !w.IsImported).ToList();

            _wi = ShowWordsInfo(unique_words, "Remove Incumbent Successor Words");

            for (int i = 0; i < unique_words.Count - 1; i++)
            {
                int j = 1;
                Word word = unique_words[i];
                Word nextWord = unique_words[i + j];

                while (
                    unique_words[i].IsImported == false &&
                    word.Lemma == unique_words[i + j].Lemma &&
                    unique_words[i + j].IsImported == false)
                {
                    nextWord = unique_words[i + j];
                    nextWord.IsImported = true;
                    word.Forms.Add(nextWord.Lemma);
                    foreach (var form in nextWord.Forms)
                    {
                        word.Forms.Add(form);
                    }

                    if (i + j < unique_words.Count - 2)
                        j++;
                }
            }

            unique_words = words.Where(w => !w.IsImported).ToList();

            _wi = ShowWordsInfo(unique_words, "Join Word Duplicates");

            HashSet<string> uniqForms = new HashSet<string>();
            foreach (Word word in unique_words)
            {
                foreach (string form in word.Forms)
                {
                    if (!uniqForms.Add(form))
                    {
                        word.Forms.Remove(form);
                    }

                }
            }

            _wi = ShowWordsInfo(unique_words, "Remove Form Duplicates");

            return unique_words;
        }

        private List<Word> ImportPredecessorForms(List<Word> words, ref int importedCount)
        {
            //get forms from Successors and Incumbents wordt to Predecessor
            foreach (Word word in words)
            {
                if (word.IsPredecessor)
                {
                    word.CopyAllSuccessorsFormsToPredecessor();
                }
            }

            foreach (Word word in words)
            {
                if ((word.IsIncumbent || word.IsSuccessor) && word.IsImported)
                {
                    importedCount++;
                }
            }

            _wi = ShowWordsInfo(words, "Import Predecessor Forms");

            return words;
        }

        private List<Word> CreateWordTypes(List<Word> words)
        {
            _wordIdComparer = new WordIdComparer();
            words.Sort(_wordIdComparer);

            //find related words, ensure nesting
            foreach (Link links in _openCorpora.Links)
            {
                Word predecessor = words[words.BinarySearch(
                    new Word(links.From), _wordIdComparer)];
                Word successor = words[words.BinarySearch(
                    new Word(links.To), _wordIdComparer)];

                predecessor.Successors.Add(successor);
                successor.Predecessors.Add(predecessor);
            }

            _wordLemmaComparer = new WordLemmaComparer();
            words.Sort(_wordLemmaComparer);

            _wti = ShowWordTypesInfo(words, "Create Word Types");

            return words;
        }

        private List<Word> CreateWords()
        {
            // convert deserialized xml to object
            List<Word> words = new List<Word>();
            foreach (Lemma lemma in _openCorpora.Lemmas)
            {
                words.Add(new Word(lemma.Id, lemma.GetLemmaType(),
                    lemma.GetLemma(), lemma.GetForms()));
            }

            words.Sort(_wordLemmaComparer);

            _wi = ShowWordsInfo(words, "Create Words");

            return words;
        }

        private WordTypesInfo ShowWordTypesInfo(List<Word> words, string title)
        {
            int nobodyCount = 0, predecessorCount = 0, incumbentCount = 0,
                successorCount = 0, totalCount = 0;
            foreach (Word word in words)
            {
                if (word.IsNobody) { nobodyCount++; continue; }
                if (word.IsPredecessor) { predecessorCount++; continue; }
                if (word.IsIncumbent) { incumbentCount++; continue; }
                if (word.IsSuccessor) { successorCount++; continue; }
            }
            totalCount = nobodyCount + predecessorCount + incumbentCount + successorCount;

            Console.WriteLine($"\n{title}\n");
            Console.WriteLine($"     Nobody    {nobodyCount}\n" +
                              $"Predecessor    {predecessorCount}\n" +
                              $"  Incumbent    {incumbentCount}\n" +
                              $"  Successor    {successorCount}\n" +
                              $"      Total    {totalCount}");

            return new WordTypesInfo(nobodyCount, predecessorCount, incumbentCount,
                successorCount, totalCount);
        }

        private WordsInfo ShowWordsInfo(List<Word> words, string title)
        {
            List<string> lemmas = new List<string>();
            List<string> forms = new List<string>();
            HashSet<string> uniqLemmas = new HashSet<string>();
            HashSet<string> uniqForms = new HashSet<string>();

            foreach (Word word in words)
            {
                lemmas.Add(word.Lemma);
                uniqLemmas.Add(word.Lemma);
                foreach (string form in word.Forms)
                {
                    forms.Add(form);
                    uniqForms.Add(form);
                }
            }

            Console.WriteLine($"\n{title}\n");
            Console.WriteLine($"     Lemmas    {lemmas.Count}\n" +
                              $" UniqLemmas    {uniqLemmas.Count}\n" +
                              $"      Forms    {forms.Count}\n" +
                              $"  UniqForms    {uniqForms.Count}");

            List<int> opencorpora_grammeme_count = new List<int>();
            List<string> opencorpora_grammeme = _openCorpora.Grammemes
                .Where(g => g.Parent.Equals("POST"))
                .Select(g => g.Name).ToList();

            foreach (var grammeme in opencorpora_grammeme)
            {
                opencorpora_grammeme_count.Add(0);
            }

            foreach (Word word in words)
            {
                for (int i = 0; i < opencorpora_grammeme.Count; i++)
                {
                    if (opencorpora_grammeme[i].Equals(word.Grammeme))
                    {
                        opencorpora_grammeme_count[i]++;
                    }
                }
            }

            Console.WriteLine();
            for (int i = 0; i < opencorpora_grammeme.Count; i++)
            {
                Console.WriteLine($"       {opencorpora_grammeme[i]}    " +
                    $"{opencorpora_grammeme_count[i]}");
            }

            return new WordsInfo(lemmas.Count, uniqLemmas.Count,
                forms.Count, uniqForms.Count);
        }
    }
}
