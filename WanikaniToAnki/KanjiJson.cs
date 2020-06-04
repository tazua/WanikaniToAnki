using System;

namespace WanikaniToAnki
{

    public class KanjiJSON
    {
        public int id { get; set; }
        public string _object { get; set; }
        public string url { get; set; }
        public DateTime? data_updated_at { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int[] amalgamation_subject_ids { get; set; }
        public Auxiliary_Meanings[] auxiliary_meanings { get; set; }
        public string characters { get; set; }
        public int[] component_subject_ids { get; set; }
        public DateTime? created_at { get; set; }
        public string document_url { get; set; }
        public object hidden_at { get; set; }
        public int lesson_position { get; set; }
        public int level { get; set; }
        public Meaning[] meanings { get; set; }
        public string meaning_hint { get; set; }
        public string meaning_mnemonic { get; set; }
        public Reading[] readings { get; set; }
        public string reading_mnemonic { get; set; }
        public string reading_hint { get; set; }
        public string slug { get; set; }
        public object[] visually_similar_subject_ids { get; set; }
    }

    public class Auxiliary_Meanings
    {
        public string meaning { get; set; }
        public string type { get; set; }
    }

    public class Meaning
    {
        public string meaning { get; set; }
        public bool primary { get; set; }
        public bool accepted_answer { get; set; }
    }

    public class Reading
    {
        public string type { get; set; }
        public bool primary { get; set; }
        public bool accepted_answer { get; set; }
        public string reading { get; set; }
    }

}
