using NUnit.Framework;
using OpenAI_API;
using OpenAI_API.Data;
using System;
using System.IO;
using System.Linq;

namespace OpenAI_Tests
{
	public class SearchEndpointTests
	{
		[SetUp]
		public void Setup()
		{
			OpenAI_API.APIAuthentication.Default = new OpenAI_API.APIAuthentication(Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY"));
		}

		[Test]
		public void TestBasicSearch()
		{
			var key = Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY");
			var auth = new OpenAI_API.APIAuthentication(key);			
			var api = new OpenAI_API.OpenAIAPI(auth, engine: Engine.Curie);

			Assert.IsNotNull(api.Search);

			var result = api.Search.GetBestMatchAsync("Washington DC"
				, "Canada"
				, "China"
				, "USA"
				, "Spain").Result;
			Assert.IsNotNull(result);
			Assert.AreEqual("USA", result);
		}
		[Test]
		public void TestFileSearch()
		{
			var key = Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY");
			var auth = new OpenAI_API.APIAuthentication(key);
			var api = new OpenAI_API.OpenAIAPI(auth, engine: Engine.Curie);

			Assert.IsNotNull(api.Search);
			SearchRequest sr = new SearchRequest
			{
				File = "file-YuUF8p1BtnYJw0E3JBrSWXf6",
				Query = "We will at all times maintain a documented data governance and protection procedure.",
				MaxRerank = 5,
				ReturnMetaData = true
			};
			var result = api.Search.GetSearchResultsAsync(sr).Result;
			Assert.IsNotNull(result);
			var topScoreMetaData = result.Results.OrderByDescending(a => a.Score).First().MetaData;
			Assert.AreEqual("Information Security Management Program", topScoreMetaData);
		}
	}
}