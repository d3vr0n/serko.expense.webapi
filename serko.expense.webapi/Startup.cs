using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using serko.expense.webapi.formatters;

namespace serko.expense.webapi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-NZ");

            Configuration = configuration;
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

            app.UseCors("CorsPolicy");

            // insert middleware to expose the generated Swagger as JSON endpoint(s)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serko expense API V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            
        }
    }
}
