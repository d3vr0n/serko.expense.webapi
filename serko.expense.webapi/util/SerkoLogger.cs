using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace serko.expense.webapi.util
{
    /// <summary>
    /// Class for Logging to console and File
    /// </summary>
    public class SerkoLogger : ISerkoLogger
    {
        protected IHttpContextAccessor _httpContextAccessor;
        public SerkoLogger(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Method to Log Information message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void LogInfo(string message, params object[] data)
        {
            string remoteUser = _httpContextAccessor?.HttpContext?.Request?.Headers["REMOTE_USER"];
            string user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            Log.Logger.Information("{@Info}", new
            {
                Environment = new
                {
                    host = System.Environment.MachineName,
                    program = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name,
                    user,
                    remoteUser
                },
                Info = new
                {
                    message,
                    data,
                }
            });
        }
        /// <summary>
        /// Method to Log Exception
        /// </summary>
        /// <param name="message">A message you want to accompany with this exception to make it easily identifiable</param>
        /// <param name="evrException">exception object</param>
        /// <param name="bizData">Do Not Pass sensitive/Personally identifiable information (PII)</param>
        public void LogException(string message, Exception evrException, object bizData)
        {
            string remoteUser = _httpContextAccessor?.HttpContext?.Request?.Headers["REMOTE_USER"];
            string user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            Log.Logger.Fatal("{@Error}",
                new
                {
                    CreatedOn = DateTime.UtcNow,
                    Environment = new
                    {
                        host = System.Environment.MachineName,
                        program = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name,
                        user,
                        remoteUser
                    },
                    Exception = new
                    {
                        message = evrException.Message,
                        data = GetSerializedBizData(bizData),
                        stackTrace = evrException.StackTrace
                    }
                });
        }

        private string GetSerializedBizData(object bizData)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                return Newtonsoft.Json.JsonConvert.SerializeObject(bizData, serializerSettings);
            }
            catch
            {
                // do not do anything
            }

            return string.Empty;
        }
    }
}
