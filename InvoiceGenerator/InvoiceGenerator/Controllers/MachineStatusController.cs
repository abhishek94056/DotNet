namespace InvoiceGenerator.Controllers
{
    using InvoiceGenerator.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace YourProject.Controllers
    {
        public class MachineStatusController : Controller
        {
            private readonly string _conn;

            public MachineStatusController(IConfiguration config)
            {
                _conn = config.GetConnectionString("InvoiceGenerator");
            }


            public ActionResult MachineStatusView(string filter = "All")
            {
                var machines = GetAllMachines();
                ViewBag.Filter = filter;
                ViewBag.AllCount = machines.Count;
                ViewBag.IdleCount = machines.Count(m => m.Status == "Idle");
                ViewBag.BreakdownCount = machines.Count(m => m.Status == "Breakdown");
                ViewBag.PreventiveCount = machines.Count(m => m.Status == "Preventive Maintenance");
                ViewBag.ShutdownCount = machines.Count(m => m.Status == "Shutdown");
                ViewBag.InUseCount = machines.Count(m => m.Status == "In-use");

                var filtered = filter == "All"
                    ? machines
                    : machines.Where(m => m.Status == filter).ToList();

                return View(filtered);
            }

            private List<MachineStatusViewModel> GetAllMachines()
            {
                var list = new List<MachineStatusViewModel>();
                using (var conn = new SqlConnection(_conn))
                {
                    conn.Open();
                    string query = @"
                    SELECT Id, MachineName, Status, Activity, WorkType,
                           ItemName, AssignedTo, Counter1, Counter2, Counter3
                    FROM MachineStatus
                    ORDER BY MachineName";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MachineStatusViewModel
                            {
                                Id = (int)reader["Id"],
                                MachineName = reader["MachineName"].ToString(),
                                Status = reader["Status"].ToString(),
                                Activity = reader["Activity"].ToString(),
                                WorkType = reader["WorkType"].ToString(),
                                ItemName = reader["ItemName"].ToString(),
                                AssignedTo = reader["AssignedTo"].ToString(),
                                Counter1 = reader["Counter1"] == DBNull.Value ? 0 : (int)reader["Counter1"],
                                Counter2 = reader["Counter2"] == DBNull.Value ? 0 : (int)reader["Counter2"],
                                Counter3 = reader["Counter3"] == DBNull.Value ? 0 : (int)reader["Counter3"],
                            });
                        }
                    }
                }
                return list;
            }
        }
    }
    
    }
