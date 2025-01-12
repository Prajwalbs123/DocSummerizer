﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using PdfRecieverAPI.Contracts;
using PdfRecieverAPI.Models;


namespace PdfRecieverAPI.Controllers
{

    /// <summary>
    ///     File Handler, upload pdf file to index (after chunking) and get pdf summary;
    /// </summary>
    /// <param name="uploadService">DI of IUploadService to handle upload logic</param>
    /// <param name="_logger">DI of Ilogger to Log information and errors in console</param>
    [Route("api/[controller]")]
	[ApiController]
	[FeatureGate("Upload")]
	public class UploadController(IUploadService uploadService, ILogger<UploadController> _logger,IFeatureManager featureManager) : ControllerBase
	{
		private readonly IUploadService uploadService = uploadService;
		private readonly ILogger<UploadController> _logger = _logger;
		private readonly IFeatureManager featureManager = featureManager;

		[HttpGet]
		public async Task<IActionResult> IsFeatureEnabled()
		{
			return Ok(await featureManager.IsEnabledAsync("Upload"));
		}
		[HttpPost("DeleteFile")]
		public async Task<IActionResult> DeleteFile(QueryModel request)
		{
            _logger.LogInformation("Deleting files...");

            return Ok(await uploadService.DeleteFileAsync(request.FileId));
		}

		/// <summary>
		///     This Endpoint processes PDF file and returns it's summary. 
		/// </summary>
		/// <param name="file">input pdf file: IFormfile</param>
		/// <returns>string: response</returns>
		[HttpPost("Summary")]
		public async Task<IActionResult> GetSummary(IFormFile file)
		{
			_logger.LogInformation("Pdf file recieved at endpoint for summarization");
			return Ok(await uploadService.GetSummaryAsync(file));
		}

		/// <summary>
		///     This EndPoint Chunks the input PDf file and upload it to azure ai search index.
		/// </summary>
		/// <param name="file">PDF file: IFormfile</param>
		/// <returns>File upload status: string</returns>

		[HttpPost("save")]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			_logger.LogInformation($"Uploading {file.FileName}");
			return Ok(await uploadService.Upload(file));
		}
	}
}
