using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using RestSharp;
using OpenAI_API.Data;

namespace OpenAI_API
{
	/// <summary>
	/// The API lets you do semantic search over documents. This means that you can provide a query, such as a natural language question or a statement, and find documents that answer the question or are semantically related to the statement. The “documents” can be words, sentences, paragraphs or even longer documents. For example, if you provide documents "White House", "hospital", "school" and query "the president", you’ll get a different similarity score for each document. The higher the similarity score, the more semantically similar the document is to the query (in this example, “White House” will be most similar to “the president”).
	/// </summary>
	public class SearchEndpoint
	{
		OpenAIAPI Api;

		/// <summary>
		/// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="OpenAIAPI"/> as <see cref="OpenAIAPI.Search"/>.
		/// </summary>
		/// <param name="api"></param>
		internal SearchEndpoint(OpenAIAPI api)
		{
			this.Api = api;
		}



		#region GetSearchResults
		/// <summary>
		/// Perform a semantic search over a list of documents
		/// </summary>
		/// <param name="request">The request containing the query and the documents to match against</param>
		/// <returns>Asynchronously returns a Dictionary mapping each document to the score for that document.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
		public async Task<SearchResponse> GetSearchResultsAsync(SearchRequest request)
		{
			if (Api.Auth?.ApiKey is null)
			{
				throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/OkGoDoIt/OpenAI-API-dotnet#authentication for details.");
			}
			var webRequest = new RestRequest($"v1/engines/{Api.UsingEngine.EngineName}/search", Method.Post);			
			var body = JsonSerializer.Serialize(request).Replace("\"documents\":[],","");
			webRequest.AddBody(body, "application/json");
			webRequest.AddHeader("Authorization", $"Bearer {Api.Auth.ApiKey}");			


			var response = await Api.Client.ExecuteAsync(webRequest);						

			if (response.IsSuccessful)
			{
				string resultAsString = response.Content;
				var res = JsonSerializer.Deserialize<SearchResponse>(resultAsString);

				try
				{
					res.Organization = response.Headers.FirstOrDefault(a => a.Name.ToLower() == "openai-organization").Value.ToString();
					res.RequestId = response.Headers.FirstOrDefault(a => a.Name.ToLower() == "x-request-id").Value.ToString();
					res.ProcessingTime = new TimeSpan(Convert.ToInt64(response.Headers.FirstOrDefault(a => a.Name.ToLower() == "openai-processing-ms").Value));
				}
				catch (Exception) { }

				if (res.Results == null || res.Results.Count == 0)
					throw new HttpRequestException("API returnes no search results.  HTTP status code: " + response.StatusCode.ToString() + ". Response body: " + resultAsString);

				return res;
			}
			else
			{
				throw new HttpRequestException("Error calling OpenAi API to get search results.  HTTP status code: " + response.StatusCode.ToString());
			}
		}



		/// <summary>
		/// Perform a semantic search of a query over a list of documents
		/// </summary>
		/// <param name="query">A query to match against</param>
		/// <param name="documents">Documents to search over, provided as a list of strings</param>
		/// <returns>Asynchronously returns a Dictionary mapping each document to the score for that document.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
		public Task<SearchResponse> GetSearchResultsAsync(string query, params string[] documents)
		{
			SearchRequest request = new SearchRequest(query, documents);
			return GetSearchResultsAsync(request);
		}

		#endregion

		#region GetBestMatch

		/// <summary>
		/// Perform a semantic search over a list of documents to get the single best match
		/// </summary>
		/// <param name="request">The request containing the query and the documents to match against</param>
		/// <returns>Asynchronously returns the best matching document</returns>
		public async Task<string> GetBestMatchAsync(SearchRequest request)
		{
			var results = await GetSearchResultsAsync(request);
			if (results.Results.Count == 0)
				return null;
			var best = results.Results.OrderByDescending(kv => kv.Score).FirstOrDefault();
			if (request.Documents == null || request.Documents.Count == 0)
			{
				return best?.MetaData ?? best?.Text;
			}
            else
            {
				var docIndex = best.DocumentIndex;
				return request.Documents[docIndex];
            }
		}

		/// <summary>
		/// Perform a semantic search over a list of documents with a specific query to get the single best match
		/// </summary>
		/// <param name="request">The request containing the documents to match against</param>
		/// <param name="query">A query to search for, overriding whatever was provided in <paramref name="request"/></param>
		/// <returns>Asynchronously returns the best matching document</returns>
		public Task<string> GetBestMatchAsync(SearchRequest request, string query)
		{
			request.Query = query;
			return GetBestMatchAsync(request);
		}

		/// <summary>
		/// Perform a semantic search of a query over a list of documents to get the single best match
		/// </summary>
		/// <param name="query">A query to match against</param>
		/// <param name="documents">Documents to search over, provided as a list of strings</param>
		/// <returns>Asynchronously returns the best matching document</returns>
		public Task<string> GetBestMatchAsync(string query, params string[] documents)
		{
			SearchRequest request = new SearchRequest(query, documents);
			return GetBestMatchAsync(request);
		}

		#endregion

		#region GetBestMatchWithScore

		/// <summary>
		/// Perform a semantic search over a list of documents to get the single best match and its score
		/// </summary>
		/// <param name="request">The request containing the query and the documents to match against</param>
		/// <returns>Asynchronously returns a tuple of the best matching document and its score.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
		public async Task<SearchResult> GetBestMatchWithScoreAsync(SearchRequest request)
		{
			var results = await GetSearchResultsAsync(request);
			var best = results.Results.OrderByDescending(kv => kv.Score).FirstOrDefault();
			return best;
		}

		/// <summary>
		/// Perform a semantic search over a list of documents with a specific query to get the single best match and its score
		/// </summary>
		/// <param name="request">The request containing the documents to match against</param>
		/// <param name="query">A query to search for, overriding whatever was provided in <paramref name="request"/></param>
		/// <returns>Asynchronously returns a tuple of the best matching document and its score.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
		public Task<SearchResult> GetBestMatchWithScoreAsync(SearchRequest request, string query)
		{
			request.Query = query;
			return GetBestMatchWithScoreAsync(request);
		}

		/// <summary>
		/// Perform a semantic search of a query over a list of documents to get the single best match and its score
		/// </summary>
		/// <param name="query">A query to match against</param>
		/// <param name="documents">Documents to search over, provided as a list of strings</param>
		/// <returns>Asynchronously returns a tuple of the best matching document and its score.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
		public Task<SearchResult> GetBestMatchWithScoreAsync(string query, params string[] documents)
		{
			SearchRequest request = new SearchRequest(query, documents);
			return GetBestMatchWithScoreAsync(request);
		}

		#endregion
	}
}
