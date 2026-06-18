using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DuoBackEnd.DTO;

namespace DuoBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IAController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public IAController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("perguntar")]
    public async Task<IActionResult> PerguntarIA([FromBody] IAQueryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Pergunta))
        {
            return BadRequest(new { erro = "A pergunta não pode estar vazia." });
        }

        // 1. Sua nova chave atualizada
        var apiKey = "AQ.Ab8RN6JdfQSv7f--9oAGq44RXevSWWG5VZp0kFra7QUkZ9mf2w"; 
        
        var client = _httpClientFactory.CreateClient();

        // 2. Formato do Prompt padrão do Gemini
        var promptFormatado = $"Contexto: Você é um assistente virtual integrado ao sistema Duo Conveniência.\nUsuário: {dto.Pergunta}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = promptFormatado }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // 3. URL CORRIGIDA: Rota unificada para chaves de nova assinatura com o modelo atual padrão
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var erroDetalhes = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { erro = "Falha na comunicação com o Gemini.", detalhes = erroDetalhes });
            }

            // 4. Extração do JSON de resposta do Google
            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            
            var respostaTexto = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new { resposta = respostaTexto });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao processar a IA.", detalhes = ex.Message });
        }
    }
}