using AIOrchestratorApp.Components;
using Syncfusion.Blazor;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Leer las claves de API desde la configuración de appsettings.json
var openAIApiKey = builder.Configuration["AISettings:OpenAIApiKey"];
var googleGeminiApiKey = builder.Configuration["AISettings:GoogleGeminiApiKey"];

// Configurar Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    bool hasOpenAI = false;
    // Añadir el modelo de OpenAI si la clave existe y no es nula/vacía
    if (!string.IsNullOrEmpty(openAIApiKey))
    {
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-4o",
            apiKey: openAIApiKey);
        hasOpenAI = true;
    }

    bool hasGemini = false;
    // Añadir el modelo de Google Gemini si la clave existe y no es nula/vacía
    if (!string.IsNullOrEmpty(googleGeminiApiKey))
    {
        kernelBuilder.AddGoogleAIGeminiChatCompletion(
            modelId: "gemini-2.0-flash-lite", 
            apiKey: googleGeminiApiKey); 
        hasGemini = true;
    }

    var kernel = kernelBuilder.Build();

    if (!hasOpenAI && !hasGemini)
    {
        throw new InvalidOperationException("No AI models were configured. Please check your API keys in appsettings.json.");
    }

    var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

    kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "DesignPlugin"), "DesignPlugin");
    kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "CodePlugin"), "CodePlugin");
    kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "RefinementPlugin"), "RefinementPlugin");
    kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "StylingPlugin"), "StylingPlugin");

    return kernel;
});



builder.Services.AddSyncfusionBlazor();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();