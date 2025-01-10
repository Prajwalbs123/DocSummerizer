using System.Text.Json;
using System.Text.Json.Nodes;
using GptDLL;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PdfRecieverAPI.Contracts;
using PdfRecieverAPI.Models;
using PdfRecieverAPI.Services;
using QuerySearchDLL;

namespace PdfRecieverAPI.Controllers
{
	/// <summary>
	/// Handles user query and used to get files stored in Index
	/// </summary>
	/// <param name="queryService">DI of IQueryService to handle query logic</param>
	/// <param name="_logger">DI of ILogger to add logging of information and errors</param>
	[Route("api/[controller]")]
	[ApiController]
	public class QueryController(IQueryService queryService, ILogger<QueryController> _logger) : ControllerBase
	{
		private readonly IQueryService queryService = queryService;
		private readonly ILogger<QueryController> _logger = _logger;

		/// <summary>
		///     Get Endpoint which
		///     returns all the files stored in specified azure search index
		/// </summary>
		/// <returns>List of files present in azure search index</returns>
		[HttpGet]
		public async Task<IActionResult> GetFileList()
		{
			_logger.LogInformation("Fetching file names");
			return Ok(await queryService.GetFileList());
		}

		/// <summary>
		///     Query request handling endpoint
		/// </summary>
		/// <param name="request">user request parsed as JsonElement</param>
		/// <returns>LLM response to user query</returns>
		[HttpPost]
		public async Task<IActionResult> Query(QueryModel request)
		{
			_logger.LogInformation("Processing Query");
			return Ok(await queryService.Query(request));
		}
	}
}
