using fnf.DocQuery.API.Contracts;
using fnf.DocQuery.API.Services;
using fnf.DocQuery.AzureSearch.Contracts;
using fnf.DocQuery.AzureSearch.Services;
using fnf.DocQuery.OpenAi.Contracts;
using fnf.DocQuery.OpenAi.Services;
using fnf.DocQuery.pdfOCR.Contracts;
using fnf.DocQuery.pdfOCR.Services;

namespace fnf.DocQuery.API.Helper
{
    internal static class ServiceRegisterar
    {

        internal static void Register(IServiceCollection services)
        {

            //API services
            services.AddSingleton<IQueryService, QueryService>();
            services.AddSingleton<IUploadService, UploadService>();

            //fnf.DocQuery.pdfOCR
            services.AddSingleton<IChunk, Chunk>();
            services.AddSingleton<IReadPdf, ReadPdf>();
            services.AddSingleton<IPdfSummary, PdfSummary>();

            //fnf.DocQuery.AzureSearch
            services.AddSingleton<ICreateIndex,CreateIndex>();
            services.AddSingleton<IUploadSearch, UploadSearch>();
            services.AddSingleton<IDeleteAzureSearchFile, DeleteAzureSearchFile>();
            services.AddSingleton<IQuerySearch, QuerySearch>();
            services.AddSingleton<IAddScoringProfile, AddScoringProfile>();
            services.AddSingleton<IAddVectorSearch, AddVectorSearch>();
            //fnf.DocQuery.OpenAi
            services.AddSingleton<IGetEmbeddings, GetEmbeddings>();
            services.AddSingleton<IGptCall, GptCall>();

           
        }
    }
}
