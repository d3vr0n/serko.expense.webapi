using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using serko.expense.biz;
using serko.expense.models;

namespace serko.expense.webapi.Controllers
{
    /// <summary>
    /// Controller for expense related calculations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseManager _expenseManager;
        public ExpenseController(IExpenseManager expenseManager)
        {
            _expenseManager = expenseManager;
        }
        /// <summary>
        /// Parse and extract an Expense form email content
        /// </summary>
        /// <response code="200">Expense created</response>
        /// <response code="400">Expense tag has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your Expense right now</response>
        [HttpPost("ParseExpenseFromEmail")]
        [ProducesResponseType(typeof(ExpenseModel), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public IActionResult GetExpenseFromEmail([FromBody] string emailBody)
        {
            if (string.IsNullOrWhiteSpace(emailBody))
            {
                return BadRequest(Constants.EMPTY_EMAIL_CONTENT);
            }

            try
            {
                return Ok(_expenseManager.ValidateAndExtract(emailBody));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception e)
            {
                // log the exception and return 500
                return StatusCode(500, Constants.GENERAL_EXCEPTION_RESPONSE);
            }
            
        }
    }
}