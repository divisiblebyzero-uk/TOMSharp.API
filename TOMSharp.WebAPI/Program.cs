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

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;

namespace TOMSharp_Loader
{
    class Program
    {
        public static void Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;


            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                // Filter out ASP.NET Core infrastructre logs that are Information and below
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.ColoredConsole(
                    LogEventLevel.Debug,
                    "{NewLine}{Timestamp:HH:mm:ss} [{Level}] [{UserName}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .WriteTo.File("logs\\tomsharpapi.log", LogEventLevel.Debug, "{NewLine}{Timestamp:HH:mm:ss} [{Level}] [{UserName}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .CreateLogger();



            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

}
