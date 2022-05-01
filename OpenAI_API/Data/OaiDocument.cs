using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAI_API.Data
{
    public class OaiDocument
    {
        public OaiDocument()
        {

        }
        public OaiDocument(string text)
        {
            Text = text;
            MetaData = text;
        }
        public OaiDocument(string text, string metadata)
        {
            Text = text;
            MetaData = metadata;
        }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("metadata")]
        public string MetaData { get; set; }
        public string SerializeToJsonl()
        {
            return $"{{\"text\": \"{Text}\", \"metadata:\": \"{MetaData}\"}}";
        }
    }
}
