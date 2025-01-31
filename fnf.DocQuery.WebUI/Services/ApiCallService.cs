﻿using fnf.DocQuery.WebUI.Contracts;
using fnf.DocQuery.WebUI.Model;

namespace fnf.DocQuery.WebUI.Services
{
    public class ApiCallService(HttpClient client) : IApiCallService
    {
        private readonly HttpClient httpClient = client;
        //upload controller
        public async Task<string> GetSummaryAsync(MultipartFormDataContent content)
        {
            var response = await httpClient.PostAsync(@"Upload/Summary", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostFileAsync(MultipartFormDataContent content)
        {
            var response = await httpClient.PostAsync(@"Upload/save", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteFile(string? fileId)
        {
            QueryModel queryModel = new QueryModel();
            queryModel.FileId = fileId;
            queryModel.Message = "Deleteing";
            queryModel.NoSentence = 1;

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(queryModel);
            HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var respose = await httpClient.PostAsync(@"Upload/DeleteFile",content);
            return await respose.Content.ReadAsStringAsync();
        }
        //query controller
        public async Task<HttpResponseMessage> PostQueryAsync(HttpContent content)
        {
            return await httpClient.PostAsync("Query", content);
        }

        public async Task<HttpResponseMessage> GetFileListAsync()
        {
            return await httpClient.GetAsync("Query");
        }
    }
}
