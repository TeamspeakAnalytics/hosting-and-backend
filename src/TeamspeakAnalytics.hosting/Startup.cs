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
      var tsCfg = Configuration.GetSection<TeamspeakConfiguration>();
      var svCfg = Configuration.GetSection<ServiceConfiguration>();

      #region Database

      var connStr = Configuration.GetConnectionString("ServiceDatabase");

      //update db
      var dbCtxOptsBuilkder = new DbContextOptionsBuilder<TS3AnalyticsDbContext>();
      dbCtxOptsBuilkder.UseSqlServer(connStr, b => b.MigrationsAssembly("TeamspeakAnalytics.database.mssql"));
      var db = new TS3AnalyticsDbContext(dbCtxOptsBuilkder.Options);

      if (db.Database.GetPendingMigrations().Any())
      {
        //TODO: log updating database
        db.Database.Migrate();
      }
      else
      {
        //TODO; log no update needed.
      }

      // add DI
      services.AddDbContext<TS3AnalyticsDbContext>(opt =>
                opt.UseSqlServer(connStr, b => b.MigrationsAssembly("TeamspeakAnalytics.database.mssql")));

      #endregion

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

      services.AddTS3Provider<CachedTS3DataProvider>(tsCfg);
      services.AddMvc()
        .AddJsonOptions(settings =>
                        {
                          settings.SerializerSettings.ContractResolver = new DefaultContractResolver
                          {
                            NamingStrategy = new CamelCaseNamingStrategy()
                          };
                        });
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
      app.UseAuthentication();
      app.UseMvc();
    }
  }
}
