using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using serko.expense.models;

namespace serko.expense.webapi.Controllers
{
    /// <summary>
    /// load balancer to nginx shall ping this service periodically to check
    /// if service is up or down
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET api/health
        [HttpGet()]
        public ActionResult<string> Get()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new HealthModel
            {
                Status = "UP",
                Timestamp = DateTime.Now.ToString(CultureInfo.CurrentCulture)
            });
        }
    }
}
