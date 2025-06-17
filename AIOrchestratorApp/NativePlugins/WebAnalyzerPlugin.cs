using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AIOrchestratorApp.NativePlugins;

/// <summary>
/// Plugin nativo para analizar contenido web.
/// Su función es visitar una URL, descargar su HTML y limpiarlo
/// para que otros agentes puedan entenderlo.
/// </summary>
public class WebAnalyzerPlugin
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebAnalyzerPlugin(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [KernelFunction, Description("Visita una URL, extrae el contenido HTML principal y lo devuelve como texto.")]
    public async Task<string> ExtractMainHtmlContent(
        [Description("La URL completa de la página web que se usará como inspiración.")] string url)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            // Añadimos un User-Agent para simular un navegador y evitar bloqueos.
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var htmlContent = await client.GetStringAsync(url);

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // Seleccionamos solo el nodo <body> para limpiar el contenido.
            var bodyNode = document.DocumentNode.SelectSingleNode("//body");

            if (bodyNode != null)
            {
                // Eliminamos etiquetas que no aportan a la estructura (scripts, estilos en línea)
                bodyNode.Descendants()
                        .Where(n => n.Name == "script" || n.Name == "style")
                        .ToList()
                        .ForEach(n => n.Remove());

                // Devolvemos el HTML limpio del cuerpo
                return bodyNode.InnerHtml;
            }

            // Si no hay body, devolvemos el HTML tal cual, pero es raro.
            return htmlContent;
        }
        catch (HttpRequestException e)
        {
            // Devolvemos un mensaje de error claro que el siguiente agente puede interpretar.
            return $"Error al acceder a la URL: {url}. Mensaje: {e.Message}";
        }
    }
}
