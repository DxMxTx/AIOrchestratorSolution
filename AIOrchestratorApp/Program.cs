using AIOrchestratorApp.Components;
using Syncfusion.Blazor;
using Microsoft.SemanticKernel; // Esta es para la clase Kernel
using Microsoft.SemanticKernel.ChatCompletion; // Para IChatCompletionService
using Microsoft.SemanticKernel.Connectors.OpenAI; // Si usas OpenAI
using Microsoft.SemanticKernel.Connectors.Google; // ¡Esta es la clave para AddGoogleAIGeminiChatCompletion!
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Leer las claves de API desde la configuración de appsettings.json
var openAIApiKey = builder.Configuration["AISettings:OpenAIApiKey"];
var googleGeminiApiKey = builder.Configuration["AISettings:GoogleGeminiApiKey"];

// Configurar Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    bool hasOpenAI = false; // <<< AÑADE ESTA LÍNEA
    // Añadir el modelo de OpenAI si la clave existe y no es nula/vacía
    if (!string.IsNullOrEmpty(openAIApiKey))
    {
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-4o", // Puedes usar "gpt-3.5-turbo", "gpt-4", o "gpt-4o"
            apiKey: openAIApiKey);
        hasOpenAI = true; // <<< Y ESTA
    }

    bool hasGemini = false; // <<< AÑADE ESTA LÍNEA
    // Añadir el modelo de Google Gemini si la clave existe y no es nula/vacía
    if (!string.IsNullOrEmpty(googleGeminiApiKey)) // ERROR: This line contains a typo `!!`
    {
#pragma warning disable SKEXP0070 // Suprime la advertencia de API experimental de Google AI
        kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId: "gemini-pro", apiKey: googleGeminiApiKey);
#pragma warning restore SKEXP0070
        hasGemini = true;
    }

    var kernel = kernelBuilder.Build();

    // Asegúrate de que al menos un modelo esté configurado. Si no hay claves, lanzará una excepción.
    if (!hasOpenAI && !hasGemini)
    {
        throw new InvalidOperationException("No AI models were configured. Please check your API keys in appsettings.json.");
    }
    // --- Fin Corrección ---

    return kernel;
});

builder.Services.AddSyncfusionBlazor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
