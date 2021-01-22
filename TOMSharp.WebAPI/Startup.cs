// Copyright (C) 2021 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader
{
    public class Startup
    {
        static readonly string _RequireAuthenticatedUserPolicy =
            "RequireAuthenticatedUserPolicy";

        private static readonly string _RequireAdminPolicy = "RequireAdminPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<TogglOptions>(Configuration.GetSection(nameof(TogglOptions)));
            services.AddScoped<TimeEntryDownloader>();
            services.AddDbContext<TogglContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TOMSharpDatabase")));
            services.AddScoped<SprintGeneratorService>();
            services.AddScoped<SpreadsheetService>();
            services.AddScoped<PeopleSheetImporter>();
            services.AddScoped<SnapshotService>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "TOMSharp API", Version = "v1"});
            });

            var authOptions = new AuthOptions();
            Configuration.GetSection(nameof(AuthOptions)).Bind(authOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.Authority = authOptions.Authority;
                opt.Audience = authOptions.Audience;
            });

            services.AddAuthorization(o =>
                {
                    o.AddPolicy(_RequireAuthenticatedUserPolicy, builder => builder.RequireAuthenticatedUser());
                    // TODO move to a group instead of a named user
                    o.AddPolicy(_RequireAdminPolicy, builder => builder.RequireUserName(Configuration["AdminUser"]));
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();
            app.UseCors("CorsPolicy");


            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TOMSharp API V1");
            });

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<SerilogRequestLogger>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(_RequireAuthenticatedUserPolicy); });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<TogglContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
