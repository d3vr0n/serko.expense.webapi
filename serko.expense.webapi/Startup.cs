using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using serko.expense.webapi.formatters;
using serko.expense.webapi.util;
using Serilog;

namespace serko.expense.webapi
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        public IServiceCollection _serviceCollection { get; set; }

        public Startup(IConfiguration configuration)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-NZ");

            _configuration = configuration;
            // setup serilog logging
            SetupSeriLog();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                // this is required for UI to call api
                var allowedDomains = new[] { "http://localhost:4200", "chrome-extension://fhbjgbiflinjbdggehcddcbncdddomop" };

                options.AddPolicy("CorsPolicy",
                    builder => builder
                        //.AllowAnyOrigin()
                        .WithOrigins(allowedDomains)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            //services.AddMvcCore().AddApiExplorer();
            
            services.AddMvc(options =>
            {
                // add input formatter to accept raw string request from body
                options.InputFormatters.Insert(0, new RawRequestBodyInputFormatter());
                // after adding InputFormatter, swagger stopped working
                // hack : https://voidnish.wordpress.com/2018/08/17/asp-net-core-odata-and-swashbuckle-workaround-for-error/
                foreach (var formatter in options.OutputFormatters
                    .OfType<RawRequestBodyInputFormatter>()
                    .Where(it => !it.SupportedMediaTypes.Any()))
                {
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("text/plain"));
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/json"));
                }
                foreach (var formatter in options.InputFormatters
                    .OfType<RawRequestBodyInputFormatter>()
                    .Where(it => !it.SupportedMediaTypes.Any()))
                {
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("text/plain"));
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/json"));
                }

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Serko Expense API",
                    Version = "v1"
                });

                c.DescribeAllEnumsAsStrings();
            });

            // register other services here
            ServiceStartup.ConfigureServices(services);
            _serviceCollection = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // use explicit cors policy
            app.UseCors("CorsPolicy");

            AddExceptionHandlingMiddleware(app);

            // insert middleware to expose the generated Swagger as JSON endpoint(s)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serko expense API V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            
        }

        /// <summary>
        /// Serilog setup
        /// </summary>
        private void SetupSeriLog()
        {
            //Configure Serilog
            var config = new LoggerConfiguration()
                .Enrich.WithProperty("App", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name)
                .Enrich.FromLogContext()
                .MinimumLevel.Warning()
                .WriteTo.Async(t => t.Console(new Serilog.Formatting.Json.JsonFormatter()));
            string pathFormat = System.Environment.GetEnvironmentVariable("LOG_PATHFORMAT");
            if (!string.IsNullOrEmpty(pathFormat))
            {
                // Wait for any queued event to be accepted by the `File` log before allowing the calling thread to resume its
                // application work after a logging call when there are 5000 LogEvents awaiting ingestion by the pipeline
                // set the buffer size to 1 to see logging file
                config.WriteTo.Async(f => f.File(new Serilog.Formatting.Json.JsonFormatter(), 
                    pathFormat, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 15),
                    bufferSize: 500, blockWhenFull: true);
            }
            Log.Logger = config.CreateLogger();
        }

        /// <summary>
        /// Method to add exception handling middleware for uncaught exceptions
        /// </summary>
        /// <param name="app"></param>
        private void AddExceptionHandlingMiddleware(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {   //intercept Unhandled Exceptions and log them
                try
                {
                    await next();
                }
                catch (Exception exp)
                {
                    // get logger instance from servicecollection
                    var logger = _serviceCollection.BuildServiceProvider().GetService<ISerkoLogger>();

                    var data = new { url = context.Request.Path };
                    logger.LogException("Unknown Error",exp, data);

                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    
                    //for production environment, Uncaught Exception is not user friendly and may contain data not for user to see.
                    var errorMessage = $"System has encountered an error. Please try again. If error persists contact support.";
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(errorMessage);
                    
                }
            });


        }
    }
}
