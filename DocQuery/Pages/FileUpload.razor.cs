using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DocQuery.Pages
{
	/// <summary>
	///		FileUpload class
	/// </summary>
	public partial class FileUpload
	{

		//JS reference
		ElementReference dropzoneElement;
		InputFile? inputFile;
		IJSObjectReference? _module, _dropZoneInstance;
		MultipartFormDataContent Content = [];

		string summaryResult = string.Empty;
		string FileText = string.Empty;
		string FileName = string.Empty;

		/// <summary>
		///		Initialization of File List
		/// </summary>
		/// <returns></returns>
		protected override void OnInitialized()
		{
            try
            {
                if (!featureService.IsUploadFeatureEnabled)
                {
                    Navigation.NavigateTo("");
                }
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error: {ex.Message}");
			}
		}

		/// <summary>
		///     Handling of JS events for file drag and drop at inital rendering
		/// </summary>
		/// <param name="firstRender"></param>
		/// <returns></returns>
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			try
            {
                if (firstRender)
				{
					
					_module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/dragDrop.js");
					_dropZoneInstance = await _module.InvokeAsync<IJSObjectReference>("startFileDropZone", dropzoneElement, inputFile?.Element);

				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex,$"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// Get summary of the selected pdf.
		/// </summary>
		private async void Get()
		{
			logger.LogInformation("Summarizing");
			try
			{
				if (Content.Count() == 0)
				{
					await JS.InvokeVoidAsync("alert", "select a new file");
				}
				else
				{
					
					summaryResult = await apiCallService.GetSummaryAsync(Content);

					Content.Dispose();
					Content = [];
					StateHasChanged();
					logger.LogInformation("Summary recieved");
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// upload the pdf file to azure search index
		/// </summary>
		private async void Send()
		{
			logger.LogInformation("Sending file to API");
			try
			{
				if (Content.Count() == 0)
				{
					await JS.InvokeVoidAsync("alert", "select a new file");
				}
				else
				{
					string ApiResponse  = await apiCallService.PostFileAsync(Content);

					Content.Dispose();
					Content = [];
					
					await JS.InvokeVoidAsync("alert",ApiResponse);
					await JS.InvokeVoidAsync("eval", "window.location.reload(true)");
				}
				logger.LogInformation($"Sent file to API");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// handling of file upload on state change at dragdrop region.
		/// </summary>
		/// <param name="e">Input file which triggers an Event</param>
		private void OnChange(InputFileChangeEventArgs e)
		{
			try
			{
				summaryResult = string.Empty;
				StateHasChanged();
				var file = e.File;
				var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 500)); // 500 MB limit
				fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
				Content.Add(fileContent, "file", file.Name);
			}
			catch(Exception ex) 
			{
				logger.LogError(ex,$"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// Disposal of all the events at the end.
		/// </summary>
		/// <returns></returns>
		public async ValueTask DisposeAsync()
		{
			try
			{
				if (_dropZoneInstance != null)
				{
					await _dropZoneInstance.InvokeVoidAsync("dispose");
					await _dropZoneInstance.DisposeAsync();
				}

				if (_module != null)
				{
					await _module.DisposeAsync();
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex,$"Error: {ex.Message}");
			}
			
		}
	}
}