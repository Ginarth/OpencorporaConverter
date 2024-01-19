using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpencorporaConverter.Classes
{
    [XmlRoot(ElementName = "dictionary")]
    public class Opencorpora
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("revision")]
        public string Revision { get; set; }

        [XmlArray(ElementName = "grammemes")]
        [XmlArrayItem(ElementName = "grammeme")]
        public List<Grammeme> Grammemes { get; set; }

        [XmlArray(ElementName = "lemmata")]
        [XmlArrayItem(ElementName = "lemma")]
        public List<Lemma> Lemmas { get; set; }

        [XmlArray(ElementName = "link_types")]
        [XmlArrayItem(ElementName = "type")]
        public List<LinkType> LinkTypes { get; set; }

        [XmlArray(ElementName = "links")]
        [XmlArrayItem(ElementName = "link")]
        public List<Link> Links { get; set; }

        public Opencorpora()
        {
            Version = "";
            Revision = "";
            Grammemes = new List<Grammeme>();
            Lemmas = new List<Lemma>();
            LinkTypes = new List<LinkType>();
            Links = new List<Link>();
        }
    }

    public class Grammeme
    {
        [XmlAttribute("parent")]
        public string Parent { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        public Grammeme()
        {
            Parent = "";
            Name = "";
        }
    }

    public class Lemma
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("rev")]
        public int Rev { get; set; }

        [XmlElement("l")]
        public L L { get; set; }

        [XmlElement("f")]
        public List<F> F { get; set; }

        public Lemma()
        {
            Id = 0;
            Rev = 0;
            L = new L();
            F = new List<F>();
        }

        public string GetLemma()
        {
            return Replace(L.T);
        }

        public HashSet<string> GetForms()
        {
            return F.Select(f => Replace(f.T)).ToHashSet();
        }

        public string GetLemmaType()
        {
            return L.G.V;
        }

        private string Replace(string text)
        {
            return text.ToLower().Replace('ё', 'е').Replace("-", "");
        }
    }

    public class L : F
    {
        [XmlElement("g")]
        public G G { get; set; }

        public L()
        {
            G = new G();
        }
    }

    public class F
    {
        [XmlAttribute("t")]
        public string T { get; set; }

        public F()
        {
            T = "";
        }
    }

    public class G
    {
        [XmlAttribute("v")]
        public string V { get; set; }

        public G()
        {
            V = "";
        }
    }

    public class LinkType
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlText]
        public string Name { get; set; }

        public LinkType()
        {
            Id = 0;
            Name = "";
        }
    }

    public class Link
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("from")]
        public int From { get; set; }

        [XmlAttribute("to")]
        public int To { get; set; }

        [XmlAttribute("type")]
        public int Type { get; set; }

        public Link()
        {
            Id = 0;
            From = 0;
            To = 0;
            Type = 0;
        }
    }
}
