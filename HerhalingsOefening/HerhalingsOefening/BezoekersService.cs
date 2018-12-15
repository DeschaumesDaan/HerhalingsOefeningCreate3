using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HerhalingsOefening
{
    public static class BezoekersService
    {
        [FunctionName("GetDagen")]
        public static async Task<IActionResult> GetDagen(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "days")] HttpRequest req,
            ILogger log)
        {
            List<string> days = new List<string>();
           

            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = "SELECT DISTINCT DagVanDeWeek FROM TijdReeksen ";
                    command.CommandText = sql;

                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        days.Add(reader["DagVanDeWeek"].ToString());
                    }
                }
            }
            return new OkObjectResult(days);
        }
    }
}
