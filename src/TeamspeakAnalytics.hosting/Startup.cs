using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      //TODO config

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "TeamspeakAnalytics - REST API", Version = "v1" });
      });

      var serverInfo = new TS3ServerInfo
      {
        QueryHostname = "127.0.0.1",
        QueryPort = 10011,
        Username = "serveradmin",
        Password = "QgDACAqj",
        ServerIndex = 1,
        ReconnectTimeout = new TimeSpan(0, 1, 0),
      };

      services.AddTS3Provider<CachedTS3DataProvider>(serverInfo);
      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      //Fire up TS3DataProvider
      app.ApplicationServices.GetService<ITS3DataProvider>();

      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamspeakAnalytics - REST API - V1");
        });
      }
      //else
      //{
      //  app.UseExceptionHandler("/Home/Error"); //TODO: define
      //}

      app.UseMvc();

      //app.UseStaticFiles();
      //app.UseMvc(routes =>
      //{
      //  routes.MapRoute(
      //      name: "default",
      //      template: "{controller=Home}/{action=Index}/{id?}");
      //});
    }
  }
}
