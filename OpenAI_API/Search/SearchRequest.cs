using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenAI_API.Data;

namespace OpenAI_API
{
	public class SearchRequest
	{

		[JsonPropertyName("documents")]
		public List<string> Documents { get; set; }
		[JsonPropertyName("file")]
		public string File { get; set; }

		[JsonPropertyName("query")]
		public string Query { get; set; }
		[JsonPropertyName("max_rerank")]
		public int MaxRerank { get; set; } = 200;

		[JsonPropertyName("return_metadata")]
		public bool ReturnMetaData { get; set; } = true;

		public SearchRequest(string query = null, params string[] documents)
		{
			Query = query;
			Documents = documents?.ToList() ?? new List<string>();
		}

		public SearchRequest(IEnumerable<string> documents)
		{
			Documents = documents.ToList();
		}
	}
}
