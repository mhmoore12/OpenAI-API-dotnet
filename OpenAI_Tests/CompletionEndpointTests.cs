using NUnit.Framework;
using OpenAI_API;
using System;
using System.IO;
using System.Linq;

namespace OpenAI_Tests
{
	public class CompletionEndpointTests
	{
		[SetUp]
		public void Setup()
		{
			var key = Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY");
			OpenAI_API.APIAuthentication.Default = new OpenAI_API.APIAuthentication(key);
		}

		[Test]
		public void GetBasicCompletion()
		{			
			var api = new OpenAI_API.OpenAIAPI( engine: Engine.Davinci);

			Assert.IsNotNull(api.Completions);
			
			var results = api.Completions.CreateCompletionsAsync(new CompletionRequest("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight", temperature: 0.1, max_tokens: 5), 5).Result;
			Assert.IsNotNull(results);
			Assert.NotNull(results.Choices);
			Assert.NotZero(results.Choices.Count);
			Assert.That(results.Choices.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
		}		
	}
}