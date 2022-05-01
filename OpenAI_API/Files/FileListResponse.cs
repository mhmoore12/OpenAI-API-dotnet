namespace OpenAI_API.Files
{
    using System.Text.Json.Serialization;
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    

    public partial class FileListResponse
    {
        [JsonPropertyName("data")]
        public List<FileDatum> Data { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
        public string Organization { get; internal set; }
        public string RequestId { get; internal set; }
        public TimeSpan ProcessingTime { get; internal set; }
    }

    public partial class FileDatum
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("bytes")]
        public long Bytes { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }
    }
}
