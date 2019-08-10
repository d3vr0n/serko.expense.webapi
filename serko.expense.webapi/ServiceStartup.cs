using System;

using Microsoft.Extensions.DependencyInjection;
using serko.expense.biz;
using serko.expense.webapi.util;

namespace serko.expense.webapi
{
    /// <summary>
    /// Register all service class here for DI
    /// https://blog.ploeh.dk/2011/07/28/CompositionRoot/
    /// </summary>
    internal static class ServiceStartup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IExpenseManager, ExpenseManager>();

            services.AddTransient<ISerkoLogger, SerkoLogger>();
        }
    }
}
