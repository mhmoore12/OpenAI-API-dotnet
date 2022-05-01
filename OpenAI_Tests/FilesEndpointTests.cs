using NUnit.Framework;
using OpenAI_API;
using OpenAI_API.Data;
using OpenAI_API.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenAI_Tests
{
	public class FilesEndpointTests
	{
		[SetUp]
		public void Setup()
		{
			OpenAI_API.APIAuthentication.Default = new OpenAI_API.APIAuthentication(Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY"));
		}

		[Test]
		public void TestFileList()
		{						
			var api = new OpenAI_API.OpenAIAPI( engine: Engine.Curie);

			Assert.IsNotNull(api.Search);

			var result = api.Files.GetListFilesAsync().Result;
			Assert.IsNotNull(result);
			var listOfDocs = result.Data.Select(a => a.Filename).ToList();
			Assert.Contains("docs.jsonl",listOfDocs);
		}
		[Test]
		public void TestFileUpload()
		{
			var api = new OpenAI_API.OpenAIAPI(engine: Engine.Curie);

			Assert.IsNotNull(api.Search);

			List<OaiDocument> docs = new List<OaiDocument>
			{
				new OaiDocument{ MetaData= "Information Security Management Program", Text = "The organization has a formal information protection program based on an accepted industry framework that is reviewed and updated as needed."},
				new OaiDocument{ MetaData= "Clear Desk and Clear Screen Policy", Text = "Covered or critical business information is not left unattended or available for unauthorized individuals to access, including on desks, printers, copiers, fax machines, and computer monitors."},
				new OaiDocument{ MetaData= "Privilege Management", Text = "Privileges are formally authorized and controlled, allocated to users on a need-to-use and event-by-event basis for their functional role (e.g. user or administrator), and documented for each system product/element."}
			};
			FileUploadRequest request = new FileUploadRequest { Documents = docs, FileName = "test.jsonl", Purpose = "search" };
			var result = api.Files.UploadDocuments(request).Result;
			Assert.IsNotNull(result);
			var purpose= result.Purpose;
			Assert.AreEqual("search", purpose);
		}		

	}
}