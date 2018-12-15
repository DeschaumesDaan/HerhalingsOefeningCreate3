using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using HerhalingsOefening.Model;
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
            try
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
            catch(Exception ex)
            {
                return new StatusCodeResult(500);
            }
           
        }

        [FunctionName("GetBezoekers")]
        public static async Task<IActionResult> GetBezoekers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bezoekers/{day}")]
            HttpRequest req, string day, ILogger log)
        {
            List<Visit> visits = new List<Visit>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT * FROM TijdReeksen WHERE DagVanDeWeek = @day";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@day", day);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            visits.Add(new Visit()
                            {
                                AantalBezoekers = int.Parse(reader["AantalBezoekers"].ToString()),
                                Dag = reader["DagVanDeWeek"].ToString(),
                                Tijdstip = int.Parse(reader["TijdstipDag"].ToString())
                            });
                        }
                    }
                }
                return new OkObjectResult(visits);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "GetBezoekersOpDag");
                return new StatusCodeResult(500);
            }
        }

    }
}
