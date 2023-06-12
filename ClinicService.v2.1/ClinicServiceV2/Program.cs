using ClinicService.Data.Context;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Kestrel
            builder.WebHost.ConfigureKestrel(opt =>
            {
                opt.Listen(IPAddress.Any, 5100, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
                opt.Listen(IPAddress.Any, 5101, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });
            #endregion

            builder.Services.AddGrpc().AddJsonTranscoding();

            #region DBContext Service
            builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });
            #endregion

            #region Configure Swagger
            // https://learn.microsoft.com/ru-ru/aspnet/core/grpc/json-transcoding-openapi?view=aspnetcore-7.0
            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "ClinicService", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, "ClinicServiceV2.xml");
                c.IncludeXmlComments(filePath);
                c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
            });
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            { 
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            app.MapGrpcService<Services.Impl.ClinicService>().EnableGrpcWeb();
            app.MapGrpcService<Services.Impl.AuthenticateService>().EnableGrpcWeb();
            app.MapGet("/", () => "Hello world!");

            app.Run();
        }
    }
}