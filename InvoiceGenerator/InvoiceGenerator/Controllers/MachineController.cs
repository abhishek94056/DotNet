using InvoiceGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace InvoiceGenerator.Controllers
{
    public class MachineController : Controller
    {
        private readonly string _conn;

        public MachineController(IConfiguration config)
        {
            _conn = config.GetConnectionString("InvoiceGenerator");
        }

        // GET: MachineSchedule
        public IActionResult MachineView()
        {
            var schedules = GetAllSchedules();
            return View(schedules);
        }

        private List<MachineModel> GetAllSchedules()
        {
            var list = new List<MachineModel>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                string query = @"
                    SELECT Id, MachineName, StartTime, EndTime, Status
                    FROM MachineSchedule
                    ORDER BY MachineName, StartTime";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MachineModel
                        {
                            Id = (int)reader["Id"],
                            MachineName = reader["MachineName"].ToString(),
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"],
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }

            return list;
        }
    }
}