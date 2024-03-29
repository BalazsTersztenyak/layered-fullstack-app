using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T1ELF0_HFT_2021222.Endpoint.Services;
using T1ELF0_HFT_2021222.Logic;
using T1ELF0_HFT_2021222.Models;
using T1ELF0_HFT_2021222.Repository;

namespace T1ELF0_HFT_2021222.Endpoint
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddSingleton<CarRentalDbContext>();

			services.AddTransient<IRepository<Car>, CarRepository>();
			services.AddTransient<IRepository<Brand>, BrandRepository>();
			services.AddTransient<IRepository<Rental>, RentalRepository>();

			services.AddTransient<CarLogic>();
			services.AddTransient<BrandLogic>();
			services.AddTransient<RentalLogic>();

			services.AddSignalR();

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "T1ELF0_HFT_2021222.Endpoint", Version = "v1" });
			});

			services.AddCors();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "T1ELF0_HFT_2021222.Endpoint v1"));
			}

			app.UseExceptionHandler(c => c.Run(async context =>
			{
				var exception = context.Features
					.Get<IExceptionHandlerPathFeature>()
					.Error;
				var response = new { Msg = exception.Message };
				await context.Response.WriteAsJsonAsync(response);
			}));

			app.UseCors(x => x
					.AllowCredentials()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.WithOrigins("http://localhost:12307"));

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<SignalRHub>("/hub");
			});

		}
	}
}
