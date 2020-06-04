using System;

namespace WanikaniToAnki
{
    public class VocabJSON
    {
        public int id { get; set; }
        public string _object { get; set; }
        public string url { get; set; }
        public DateTime data_updated_at { get; set; }
        public Data3 data { get; set; }
    }

    public class Data3
    {
        public Auxiliary_Meanings2[] auxiliary_meanings { get; set; }
        public string characters { get; set; }
        public int[] component_subject_ids { get; set; }
        public Context_Sentences[] context_sentences { get; set; }
        public DateTime created_at { get; set; }
        public string document_url { get; set; }
        public object hidden_at { get; set; }
        public int lesson_position { get; set; }
        public int level { get; set; }
        public Meaning2[] meanings { get; set; }
        public string meaning_mnemonic { get; set; }
        public string[] parts_of_speech { get; set; }
        public Pronunciation_Audios[] pronunciation_audios { get; set; }
        public Reading2[] readings { get; set; }
        public string reading_mnemonic { get; set; }
        public string slug { get; set; }
    }

    public class Auxiliary_Meanings2
    {
        public string type { get; set; }
        public string meaning { get; set; }
    }

    public class Context_Sentences
    {
        public string en { get; set; }
        public string ja { get; set; }
    }

    public class Meaning2
    {
        public string meaning { get; set; }
        public bool primary { get; set; }
        public bool accepted_answer { get; set; }
    }

    public class Pronunciation_Audios
    {
        public string url { get; set; }
        public Metadata metadata { get; set; }
        public string content_type { get; set; }
    }

    public class Metadata
    {
        public string gender { get; set; }
        public int source_id { get; set; }
        public string pronunciation { get; set; }
        public int voice_actor_id { get; set; }
        public string voice_actor_name { get; set; }
        public string voice_description { get; set; }
    }

    public class Reading2
    {
        public bool primary { get; set; }
        public string reading { get; set; }
        public bool accepted_answer { get; set; }
    }

}
