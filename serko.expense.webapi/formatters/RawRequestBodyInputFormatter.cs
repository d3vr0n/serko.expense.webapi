using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace serko.expense.webapi.formatters
{
    /// <summary>
    /// Formatter class to accept raw stream
    /// https://github.com/kashifsoofi/DotNetSandbox/blob/master/RawRequestBodySample/Formatters/RawRequestBodyInputFormatter.cs
    /// </summary>
    public class RawRequestBodyInputFormatter : InputFormatter
    {
        public override bool CanRead(InputFormatterContext context)
        {
            return (context.HttpContext.Request.ContentType != "application/json");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var request = context.HttpContext.Request;
            using (var reader = new StreamReader(request.Body))
            {
                try
                {
                    var content = await reader.ReadToEndAsync();
                    return await InputFormatterResult.SuccessAsync(content);
                }
                catch
                {
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
