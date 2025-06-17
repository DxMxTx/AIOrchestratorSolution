using AIOrchestratorApp.Components;
using AIOrchestratorApp.NativePlugins;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

// Leer las claves de API desde la configuración de appsettings.json
var openAIApiKey = builder.Configuration["AISettings:OpenAIApiKey"];
var googleGeminiApiKey = builder.Configuration["AISettings:GoogleGeminiApiKey"];

// Soporte para HttpClientFactory, necesario para el WebAnalyzerPlugin
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WebAnalyzerPlugin>();

// Configurar Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    // --- PASO 1: AÑADIR LOS SERVICIOS DE IA ---
    bool hasModel = false;
    if (!string.IsNullOrEmpty(openAIApiKey))
    {
        kernelBuilder.AddOpenAIChatCompletion(modelId: "gpt-4o", apiKey: openAIApiKey);
        hasModel = true;
    }

    if (!string.IsNullOrEmpty(googleGeminiApiKey))
    {
        kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId: "gemini-1.5-flash", apiKey: googleGeminiApiKey);
        hasModel = true;
    }

    if (!hasModel)
    {
        throw new InvalidOperationException("No se configuró ningún modelo de IA. Revisa tus claves de API en appsettings.json.");
    }


    // Añadir el plugin nativo
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebAnalyzerPlugin>());

    // Añadir los plugins de prompts
    var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
    kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(pluginsDirectory, "DesignPlugin"), "DesignPlugin");
    kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(pluginsDirectory, "StylingPlugin"), "StylingPlugin");
    kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(pluginsDirectory, "CodePlugin"), "CodePlugin");
    kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(pluginsDirectory, "RefinementPlugin"), "RefinementPlugin");

    // --- PASO 3: CONSTRUIR EL KERNEL UNA SOLA VEZ, AL FINAL ---
    return kernelBuilder.Build();
});

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
