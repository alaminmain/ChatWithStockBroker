using Mscc.GenerativeAI;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StockMarket.Api.Services
{
    public class GeminiService
    {
        private readonly GenerativeModel _model;

        public GeminiService(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiApiKey"];
            var googleAI = new GoogleAI(apiKey: apiKey);
            _model = googleAI.GenerativeModel(model: "gemini-pro");
        }

        public async Task<string> AnalyzeFinancialData(string text)
        {
            var prompt = $@"
Please analyze the following financial data and provide a summary in JSON format.
The JSON object should have two properties: 'statementType' and 'entries'.
'statementType' should be a string (e.g., 'Income Statement', 'Balance Sheet').
'entries' should be an array of objects, where each object has 'standardAccountName', 'originalAccountName', and 'value'.

- 'standardAccountName': The standardized name of the account.
- 'originalAccountName': The account name as it appears in the document.
- 'value': The numeric value of the account.

Here is the financial data:

{text}
";
            var response = await _model.GenerateContent(prompt);
            return response.Text ?? string.Empty;
        }
    }
}
