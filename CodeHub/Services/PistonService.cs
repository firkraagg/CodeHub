using System.Text;
using System.Text.Json;

namespace CodeHub.Services
{
    public class PistonService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> ExecuteCodeAsync(string language, string version, string code)
        {
            string fileName = language.ToLower() switch
            {
                "java" => "Main.java",
                "c" => "main.c",
                "c++" => "main.cpp",
                "csharp" => "Program.cs",
                _ => "main.txt"
            };

            var requestBody = new
            {
                language = language.ToLower(),
                version = version,
                files = new[]
                {
                    new { name = fileName, content = code, encoding = "utf8" }
                },
                stdin = "",
                args = Array.Empty<string>(),
                compile_timeout = 10000,
                run_timeout = 3000,
                compile_cpu_time = 10000,
                run_cpu_time = 3000,
                compile_memory_limit = -1,
                run_memory_limit = -1
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("https://emkc.org/api/v2/piston/execute", content);

            if (!response.IsSuccessStatusCode)
                return "Error: " + response.StatusCode;

            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
