using DocQuery.Model;

namespace DocQuery.Pages
{
    public partial class UserQuery
    {
        private string messageInput = string.Empty;
        private int noSentence = 5;
        string? fileName;

        /// <summary>
        /// Regeneration of LLM response for particular query using it's index
        /// </summary>
        /// <param name="i">index of the query for which the response has to be regenerated</param>
        /// <returns></returns>
        private async Task Regenerate(int i)
        {
            logger.LogInformation("Regenerating response.....");
            try
            {
				if (i < Math.Min(SharedDataModel.Messages.Count, SharedDataModel.Responses.Count))
				{
                    QueryModel queryModel = new QueryModel();
                    queryModel.Message = SharedDataModel.Messages[i];
                    queryModel.NoSentence = noSentence;
                    queryModel.FileName = fileName;

                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(queryModel);
					HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

					var response = await apiCallService.PostQueryAsync(content);

					if (response.IsSuccessStatusCode)
					{
						var res = await response.Content.ReadAsStringAsync();
						var formattedres = res.Replace("\n", "<br>");
                        SharedDataModel.Responses[i] = formattedres;
                        SharedDataModel.datetime[i] = DateTime.UtcNow.AddHours(5.5).ToString("hh:mm:ss tt, dd-MM-yyyy");
					}
					else
					{
                        SharedDataModel.Responses[i] = "failed to get response from LLM";
						SharedDataModel.datetime[i] = DateTime.UtcNow.AddHours(5.5).ToString("hh:mm:ss tt, dd-MM-yyyy");
					}
                    logger.LogInformation("Regenration completed");
                }
                else
                {
                    throw new IndexOutOfRangeException("Index out of range");
                }
			}
            catch (Exception ex)
            {
                logger.LogError($"Error: {ex.Message}");
            }
            
        }

        /// <summary>
        ///     Send user query to LLM via API
        /// </summary>
        /// <returns></returns>
        private async Task SendMessage()
        {
            logger.LogInformation("Generating Response for query");
            try
            {
				if (!string.IsNullOrWhiteSpace(messageInput))
				{

                    SharedDataModel.Messages.Add(messageInput);
                    //invoke ai
                    QueryModel queryModel = new QueryModel();
                    queryModel.Message = messageInput;
                    queryModel.NoSentence = noSentence;
                    queryModel.FileName = fileName;

					var jsonContent = System.Text.Json.JsonSerializer.Serialize(queryModel);
					HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");


					messageInput = string.Empty;
					var response = await apiCallService.PostQueryAsync(content);

					if (response.IsSuccessStatusCode)
					{
						var res = await response.Content.ReadAsStringAsync();
						var formattedres = res.Replace("\n", "<br>");
						SharedDataModel.Responses.Add(formattedres);
						SharedDataModel.datetime.Add(DateTime.UtcNow.AddHours(5.5).ToString("hh:mm:ss tt, dd-MM-yyyy"));
						logger.LogInformation($"Response received from API/LLM");
					}
					else
					{
						SharedDataModel.Responses.Add("failed to get response from LLM");
						SharedDataModel.datetime.Add(DateTime.UtcNow.AddHours(5.5).ToString("hh:mm:ss tt, dd-MM-yyyy"));
                        throw new Exception("failed to get response from LLM");
					}
                }
                else
                {
                    throw new Exception("Null Query");
                }
			}
            catch (Exception ex)
            {
                logger.LogError($"Error {ex.Message}");
            }
            
        }
    }
}