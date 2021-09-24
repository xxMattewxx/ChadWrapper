using System;
using System.IO;
using System.Net;

using ChadWrapper.Boinc;
using ChadWrapper.Discord;
using ChadWrapper.Responses;
using ChadWrapper.Requests.Tasks;
using ChadWrapper.Responses.Tasks;
using System.Text;
using System.Collections.Generic;
using MySqlConnector;

namespace ChadWrapper.Handlers.Tasks
{
    class GenSequential
    {
        private static string InsertBase = @"INSERT INTO workunit (create_time, appid, name, xml_doc, 
            batch, rsc_fpops_est, rsc_fpops_bound, rsc_memory_bound, rsc_disk_bound, 
            need_validate, canonical_resultid, canonical_credit, transition_time, 
            delay_bound, error_mask, file_delete_state, assimilate_state, hr_class, 
            opaque, min_quorum, target_nresults, max_error_results, max_total_results, 
            max_success_results, result_template_file, priority, mod_time, rsc_bandwidth_bound, 
            fileset_id, app_version_id, transitioner_flags, size_class, keywords, app_version_num
        ) VALUES ";

        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            string APIKey = context.Request.Headers.Get("Authorization");

            if (APIKey == null || APIKey.Length != 64)
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid API key" }.ToJSON());
                return;
            }

            AddSequentialRequest request = AddSequentialRequest.FromJSON(jsonStr);
            if (request == null || !request.IsValid())
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid request" }.ToJSON());
                return;
            }

            AppInfo app = AppInfo.GetFromCodename(request.Codename);
            if (app == null)
            {
                writer.WriteLine(new BaseResponse() { Message = "App codename not found." }.ToJSON());
                return;
            }

            if (Global.AlertsWebhookURL != null)
            {
                Webhook.SendMessage(Global.AlertsWebhookURL, string.Format(
                    "Sequential workgen running for app {0}\n" +
                    "Start: {1} | End: {2} | Task Size: {3}",

                    request.Codename,
                    request.Start,
                    request.End,
                    request.TaskSize
                ));
            }

            DateTime startTime = DateTime.Now;
            Int64 tasksCount = 0;
            Int64 currOffset = request.Start;

            //batch send tasks
            List<string> rowBuffer = new List<string>();
            while(currOffset + request.TaskSize <= request.End)
            {
                string rowValues = GenerateRow(app.ID, request.Codename, request.Start / request.TaskSize + tasksCount, currOffset, Math.Min(currOffset + request.TaskSize, request.End));

                rowBuffer.Add(rowValues);
                if (rowBuffer.Count == 10000 && !InsertBuffer(rowBuffer))
                {
                    writer.WriteLine(new BaseResponse()
                    {
                        Message = "Something went wrong adding workunits to BOINC."
                    }.ToJSON());
                    return;
                }

                tasksCount++;
                currOffset += request.TaskSize;
            }

            if (rowBuffer.Count > 0 && !InsertBuffer(rowBuffer))
            {
                writer.WriteLine(new BaseResponse()
                {
                    Message = "Something went wrong adding workunits to BOINC."
                }.ToJSON());
                return;
            }

            double timeElapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;

            writer.WriteLine(new AddSequentialResponse()
            {
                Success = true,
                Message = "Tasks generated successfully.",
                TasksCount = tasksCount
            }.ToJSON());

            if (Global.AlertsWebhookURL != null)
            {
                Webhook.SendMessage(Global.AlertsWebhookURL, string.Format(
                    "Sequential workgen finished running for app {0}.\n" +
                    "-> {1} tasks created.\n" +
                    "-> {2} ms execution time.\n",

                    request.Codename,
                    tasksCount,
                    timeElapsed
                ));
            }
        }

        public static string GenerateRow(Int64 appID, string codename, Int64 taskID, Int64 start, Int64 end)
        {
            string xml = GenerateXMLData(taskID, start, end);
            //dear BOINC, this is ridiculous. Please.
            //you're better than that.
            //like...
            //..../-.....--/.....-/-/...-..
            //...../.../.../.....-..--./
            return string.Format(
                $@"(
{Utils.ConvertToUnixTimestamp(DateTime.Now)},{appID},
'chad_{codename}_{start}_{end}','{xml}',0,3e16,6e20,500000000,1000000000,0,0,666,
{Utils.ConvertToUnixTimestamp(DateTime.Now)},1209600,0,0,0,0,0,2,2,
3,10,6,'templates/chadwrapper_out',666,NOW(),0,0,0,0,-1,'',0)"
            );
        }

        public static string GenerateXMLData(Int64 taskID, Int64 start, Int64 end)
        {
            return $"<workunit><command_line>--task {taskID} --start {start} --end {end}</command_line></workunit>";
        }

        public static bool InsertBuffer(List<string> buffer)
        {
            try
            {
                StringBuilder finalStr = new StringBuilder();
                finalStr.Append(string.Join(',', buffer));

                using (MySqlConnection connection = new MySqlConnection(Global.BoincDBConnectionBuilder.ConnectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = InsertBase + finalStr.ToString() + ";";
                        command.ExecuteNonQuery();
                    }
                }
                buffer.Clear();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
