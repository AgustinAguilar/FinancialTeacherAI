using FinancialTeacherAI.Services;
using FinancialTeacherAI.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace FinancialTeacherAI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var apiKey = Configuration["AzureOpenAIChatCompletion:ApiKey"];
            var endpoint = Configuration["AzureOpenAIChatCompletion:Endpoint"];
            var chatDeploymentName = Configuration["AzureOpenAIChatCompletion:DeploymentName"];

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinancialTeacherAI API", Version = "v1" });
            });

            services.AddSingleton<IChatCompletionService>(sp =>
            {
                return new AzureOpenAIChatCompletionService(chatDeploymentName!, endpoint!, apiKey!);
            });

#pragma warning disable SKEXP0010
            

            services.AddAzureOpenAITextEmbeddingGeneration(
                deploymentName: "text-embedding-ada-002",
                endpoint,
                apiKey
            );

            services.AddKeyedTransient("FinancialAIKernel", (sp, key) =>
            {
                // Create a collection of plugins that the kernel will use
                KernelPluginCollection pluginCollection = new();
                return new Kernel(sp, pluginCollection);
            });

            // Register services for dependency injection
            services.AddScoped<IPineconeService, PineconeService>();
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<IFinancialAIService, FinancialAIService>();
            services.AddScoped<IHuggingFaceService, HuggingFaceService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinancialTeacherAI API v1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}