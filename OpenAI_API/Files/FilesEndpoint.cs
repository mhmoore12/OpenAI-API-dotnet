using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using RestSharp;

namespace OpenAI_API.Files
{
    public class FilesEndpoint
    {
        OpenAIAPI Api;

        /// <summary>
        /// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="OpenAIAPI"/> as <see cref="OpenAIAPI.Search"/>.
        /// </summary>
        /// <param name="api"></param>
        internal FilesEndpoint(OpenAIAPI api)
        {
            this.Api = api;
        }
        public async Task<FileListResponse> GetListFilesAsync()
        {
            if (Api.Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/OkGoDoIt/OpenAI-API-dotnet#authentication for details.");
            }

            var webRequest = new RestRequest("v1/files", Method.Get);
            webRequest.AddHeader("Authorization", $"Bearer {Api.Auth.ApiKey}");


            
            var response = await Api.Client.GetAsync(webRequest);
            if (response.IsSuccessful)
            {
                string resultAsString = response.Content;
                var res = JsonSerializer.Deserialize<FileListResponse>(resultAsString);

                try
                {
                    res.Organization = response.Headers.FirstOrDefault(a => a.Name == "OpenAI-Organization").Value.ToString();
                    res.RequestId = response.Headers.FirstOrDefault(a => a.Name == "X-Request-ID").Value.ToString();
                    res.ProcessingTime = new TimeSpan(Convert.ToInt64(response.Headers.FirstOrDefault(a => a.Name == "OpenAI-Processing-Ms").Value));
                }
                catch (Exception) { }

                if (res.Data == null || res.Data.Count == 0)
                    throw new HttpRequestException("API returned no file list results.  HTTP status code: " + response.StatusCode.ToString() + ". Response body: " + resultAsString);

                return res;
            }
            else
            {
                throw new HttpRequestException("Error calling OpenAi API to get File List.  HTTP status code: " + response.StatusCode.ToString() + ".");
            }
        }
        /// <summary>
        /// Perform a semantic search over a list of documents
        /// </summary>
        /// <param name="request">The request containing the query and the documents to match against</param>
        /// <returns>Asynchronously returns a Dictionary mapping each document to the score for that document.  The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher), where a score above 200 usually means the document is semantically similar to the query.</returns>
        public async Task<FileUploadResponse> UploadDocuments(FileUploadRequest request)
        {
            if (Api.Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.  Please refer to https://github.com/OkGoDoIt/OpenAI-API-dotnet#authentication for details.");
            }
            string tempPath = Path.GetTempPath();
            var fileName = tempPath + request.FileName;
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var doc in request.Documents)
                {
                    sw.WriteLine(doc.SerializeToJsonl());
                }
            }


            var webRequest = new RestRequest("v1/files", Method.Post);
            webRequest.AddHeader("Authorization", $"Bearer {Api.Auth.ApiKey}");

            webRequest.AddParameter("purpose", "search");
            webRequest.AddFile("file", fileName);        

            var response = await Api.Client.ExecuteAsync(webRequest);
            if (response.IsSuccessful)
            {
                File.Delete(fileName);
                string resultAsString = response.Content;
                var res = JsonSerializer.Deserialize<FileUploadResponse>(resultAsString);

                try
                {
                    res.Organization = response.Headers.FirstOrDefault(a => a.Name == "OpenAI-Organization").Value.ToString();
                    res.RequestId = response.Headers.FirstOrDefault(a => a.Name == "X-Request-ID").Value.ToString();
                    res.ProcessingTime = new TimeSpan(Convert.ToInt64(response.Headers.FirstOrDefault(a => a.Name == "OpenAI-Processing-Ms").Value));
                }
                catch (Exception) { }

                return res;
            }
            else
            {
                throw new HttpRequestException("Error calling OpenAi API to post files.  HTTP status code: " + response.StatusCode.ToString());
            }

        }
    }
}
