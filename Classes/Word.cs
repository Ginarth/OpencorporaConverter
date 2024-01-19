using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpencorporaConverter.Classes
{
    internal class Word
    {
        public int Id { get => _id; }
        public string Grammeme { get => _grammeme; }
        public string Lemma { get => _lemma; }
        public HashSet<string> Forms { get => _forms; set => _forms = value; }
        public List<Word> Predecessors { get => _predecessors; set => _predecessors = value; }
        public List<Word> Successors { get => _successors; set => _successors = value; }

        public bool IsNobody
        {
            get => _predecessors.Count == 0 &&
                   _successors.Count == 0;
        }
        public bool IsPredecessor
        {
            get => _predecessors.Count == 0 &&
                   _successors.Count > 0;
        }

        public bool IsIncumbent
        {
            get => _predecessors.Count > 0 &&
                   _successors.Count > 0;
        }

        public bool IsSuccessor
        {
            get => _predecessors.Count > 0 &&
                   _successors.Count == 0;
        }

        public bool IsImported { get => _isImported; set => _isImported = value; }

        private int _id;
        private string _grammeme;
        private string _lemma;
        private HashSet<string> _forms;
        private List<Word> _predecessors;
        private List<Word> _successors;
        private bool _isImported;

        public Word(int id, string grammeme, string lemma, HashSet<string> forms)
        {
            _id = id;
            _grammeme = grammeme;
            _lemma = lemma;
            _forms = forms;
            _predecessors = new List<Word>();
            _successors = new List<Word>();
            _isImported = false;
        }

        public Word(int id)
        {
            _id = id;
            _grammeme = "";
            _lemma = "";
            _forms = new HashSet<string>();
            _predecessors = new List<Word>();
            _successors = new List<Word>();
            _isImported = false;
        }

        public void CopyAllSuccessorsFormsToPredecessor()
        {
            if (IsPredecessor)
            {
                foreach (Word successor in _successors)
                {
                    foreach (string form in successor.GetAllSuccessorForms())
                    {
                        Forms.Add(form);
                    }
                }
            }
        }

        private HashSet<string> GetAllSuccessorForms()
        {
            HashSet<string> forms = new HashSet<string>();

            if (IsIncumbent || IsSuccessor)
            {
                _isImported = true;
                forms.Add(Lemma);
                foreach (var form in _forms)
                {
                    forms.Add(form);
                }
            }

            if (IsIncumbent)
            {
                foreach (Word successor in _successors)
                {
                    foreach (string form in successor.GetAllSuccessorForms())
                    {
                        forms.Add(form);
                    }
                }
            }

            return forms;
        }
    }

    internal class WordIdComparer : IComparer<Word>
    {
        public int Compare(Word? x, Word? y)
        {
            return x!.Id.CompareTo(y!.Id);
        }
    }

    internal class WordLemmaComparer : IComparer<Word>
    {
        public int Compare(Word? x, Word? y)
        {
            return x!.Lemma.CompareTo(y!.Lemma);
        }
    }
}
