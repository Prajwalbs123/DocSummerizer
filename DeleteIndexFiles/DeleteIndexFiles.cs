using Microsoft.Extensions.Configuration;

namespace DeleteIndexFilesDLL
{
    public class DeleteIndexFiles(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> DeleteAllData()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

    }
}
