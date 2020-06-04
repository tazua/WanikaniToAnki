namespace QuickType
{
    using Newtonsoft.Json;
    using System;

    public partial class Subjects
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("pages")]
        public Pages Pages { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        [JsonProperty("data_updated_at")]
        public DateTimeOffset DataUpdatedAt { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("data_updated_at")]
        public DateTimeOffset DataUpdatedAt { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("hidden_at")]
        public object HiddenAt { get; set; }

        [JsonProperty("document_url")]
        public Uri DocumentUrl { get; set; }

        [JsonProperty("characters")]
        public string Characters { get; set; }

        [JsonProperty("meanings")]
        public Meaning[] Meanings { get; set; }

        [JsonProperty("readings")]
        public Reading[] Readings { get; set; }

        [JsonProperty("component_subject_ids")]
        public long[] ComponentSubjectIds { get; set; }

        [JsonProperty("amalgamation_subject_ids")]
        public long[] AmalgamationSubjectIds { get; set; }

        [JsonProperty("visually_similar_subject_ids")]
        public object[] VisuallySimilarSubjectIds { get; set; }

        [JsonProperty("meaning_mnemonic")]
        public string MeaningMnemonic { get; set; }

        [JsonProperty("meaning_hint")]
        public string MeaningHint { get; set; }

        [JsonProperty("reading_mnemonic")]
        public string ReadingMnemonic { get; set; }

        [JsonProperty("reading_hint")]
        public string ReadingHint { get; set; }

        [JsonProperty("lesson_position")]
        public long LessonPosition { get; set; }
    }

    public partial class Meaning
    {
        [JsonProperty("meaning")]
        public string MeaningMeaning { get; set; }

        [JsonProperty("primary")]
        public bool Primary { get; set; }

        [JsonProperty("accepted_answer")]
        public bool AcceptedAnswer { get; set; }
    }

    public partial class Reading
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("primary")]
        public bool Primary { get; set; }

        [JsonProperty("accepted_answer")]
        public bool AcceptedAnswer { get; set; }

        [JsonProperty("reading")]
        public string ReadingReading { get; set; }
    }

    public partial class Pages
    {
        [JsonProperty("per_page")]
        public long PerPage { get; set; }

        [JsonProperty("next_url")]
        public Uri NextUrl { get; set; }

        [JsonProperty("previous_url")]
        public object PreviousUrl { get; set; }
    }

    public partial class Assignements
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("pages")]
        public Pages2 Pages { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        [JsonProperty("data_updated_at")]
        public DateTimeOffset? DataUpdatedAt { get; set; }

        [JsonProperty("data")]
        public Datum2[] Data { get; set; }
    }

    public partial class Datum2
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("data_updated_at")]
        public DateTimeOffset? DataUpdatedAt { get; set; }

        [JsonProperty("data")]
        public Data2 Data { get; set; }
    }

    public partial class Data2
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("subject_id")]
        public long SubjectId { get; set; }

        [JsonProperty("subject_type")]
        public string SubjectType { get; set; }

        [JsonProperty("srs_stage")]
        public long SrsStage { get; set; }

        [JsonProperty("srs_stage_name")]
        public string SrsStageName { get; set; }

        [JsonProperty("unlocked_at")]
        public DateTimeOffset? UnlockedAt { get; set; }

        [JsonProperty("started_at")]
        public DateTimeOffset? StartedAt { get; set; }

        [JsonProperty("passed_at")]
        public DateTimeOffset? PassedAt { get; set; }

        [JsonProperty("burned_at")]
        public object BurnedAt { get; set; }

        [JsonProperty("available_at")]
        public DateTimeOffset? AvailableAt { get; set; }

        [JsonProperty("resurrected_at")]
        public object ResurrectedAt { get; set; }

        [JsonProperty("passed")]
        public bool Passed { get; set; }
    }

    public partial class Pages2
    {
        [JsonProperty("per_page")]
        public long PerPage { get; set; }

        [JsonProperty("next_url")]
        public Uri NextUrl { get; set; }

        [JsonProperty("previous_url")]
        public object PreviousUrl { get; set; }
    }
}
