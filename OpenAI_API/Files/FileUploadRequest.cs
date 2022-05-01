namespace OpenAI_API.Files
{
    using OpenAI_API.Data;
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Text.Json.Serialization;

    public partial class FileUploadRequest
    {
        public FileUploadRequest()
        {
            Documents = new List<OaiDocument>();
        }
        [JsonPropertyName("purpose")]
        public string Purpose { get; set; } 
        [JsonIgnore]
        public string FileName { get; set; }
        public List<OaiDocument> Documents { get; set; }
    }
}
