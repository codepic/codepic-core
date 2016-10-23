using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Codepic.Core.Mvc
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"Hosted by {Program.Server}\r\n\r\n");

                switch(context.Request.Path){
                    case "/env":

                        foreach(var envVar in Configuration.GetChildren())
                        {
                            await context.Response.WriteAsync($"{envVar.Key}: {envVar.Value}\r\n\r\n");
                        }
                    break;
                }
                
                if (serverAddressesFeature != null)
                {
                    await context.Response.WriteAsync($"Listening on: {string.Join(", ", serverAddressesFeature.Addresses)}\r\n\r\n");
                }

                await context.Response.WriteAsync($"Served on: {context.Request.GetDisplayUrl()}\r\n\r\n");
            });
        }
    }
}