using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using TeamspeakAnalytics.ts3provider;
using TeamspeakAnalytics.hosting.Helper;
using TeamspeakAnalytics.hosting.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamspeakAnalytics.database.mssql;
using Microsoft.Extensions.Hosting;
using TeamspeakAnalytics.ts3provider.TS3DataProviders;
using TeamspeakAnalytics.hosting.Jobs;

namespace TeamspeakAnalytics.hosting
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      var tsCfg = Configuration.GetSection<TeamspeakConfiguration>();
      var svCfg = Configuration.GetSection<ServiceConfiguration>();
      services.AddSingleton<ServiceConfiguration>(svCfg);

      services.AddCors(o => o.AddPolicy("DevPolicy", builder =>
      {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
      }));

      services.AddDbContext<TS3AnalyticsDbContext>(opt =>
          opt.UseSqlServer(Configuration.GetConnectionString("ServiceDatabase"),
                            b => b.MigrationsAssembly("TeamspeakAnalytics.database.mssql")));

      services.AddSingleton<IHostedService, AnalyticsJob>();
      services.AddSingleton<IHostedService, AggregationJobs>();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "TeamspeakAnalytics - REST API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field (Like \"Bearer asdad22...\")", Name = "Authorization", Type = "apiKey" });
        c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                { "Bearer", Enumerable.Empty<string>() },
            });
      });

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = svCfg.Hostname,
            ValidAudience = svCfg.Hostname,
            IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(svCfg.SecurityKey))
          };
        });

      services.AddTS3Provider<LiveTS3DataProvider>(tsCfg);
      services.AddMvc()
        .AddJsonOptions(settings =>
                        {
                          settings.SerializerSettings.ContractResolver = new DefaultContractResolver
                          {
                            NamingStrategy = new CamelCaseNamingStrategy()
                          };
                        });
    }

    public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
    {
      //Fire up TS3DataProvider
      app.ApplicationServices.GetService<ITS3DataProvider>();

      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
        app.UseCors("DevPolicy");

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamspeakAnalytics - REST API - V1");
        });
      }
      app.UseAuthentication();
      app.UseMvc();
    }
  }
}
