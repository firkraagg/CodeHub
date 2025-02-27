using System.Diagnostics;

namespace CodeHub.Services
{
    public class CodeExecutionService
    {
        public async Task<string> ExecuteCSharpCodeAsync(string userCode)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            string projectPath = Path.Combine(tempPath, "cs_project");

            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
                string initCommand =
                    $"docker run --rm -v \"{tempPath.Replace("\\", "/")}:/app\" mcr.microsoft.com/dotnet/sdk:9.0 sh -c " +
                    $"\"dotnet new console -o /app/cs_project\"";

                await RunCommandAsync(initCommand);
            }

            string codeFilePath = Path.Combine(projectPath, "Program.cs");
            await File.WriteAllTextAsync(codeFilePath, userCode);

            string runCommand =
                $"docker run --rm -v \"{tempPath.Replace("\\", "/")}:/app\" mcr.microsoft.com/dotnet/sdk:9.0 sh -c " +
                $"\"dotnet build /app/cs_project -c Debug && dotnet run --project /app/cs_project\"";



            return await RunCommandAsync(runCommand);
        }

        public async Task<string> ExecuteJavaCodeAsync(string userCode)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempPath);

            string codeFilePath = Path.Combine(tempPath, "Main.java");
            await File.WriteAllTextAsync(codeFilePath, userCode);

            string containerCommand =
                $"docker run --rm " +
                $"--memory=128m --cpus=0.5 " +
                $"--security-opt=no-new-privileges " +
                $"--network=none " +
                $"-v \"{tempPath.Replace("\\", "/")}:/app:ro\" " +
                $"-v \"{tempPath}/output:/output\" " + 
                $"openjdk:21 sh -c \"timeout 5 javac /app/Main.java -d /output && timeout 5 java -cp /output Main\"";

            return await RunCommandAsync(containerCommand, tempPath);
        }

        private async Task<string> RunCommandAsync(string command, string tempPath = null)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result;

            try
            {
                var waitForExitTask = process.WaitForExitAsync();
                if (await Task.WhenAny(waitForExitTask, Task.Delay(TimeSpan.FromSeconds(10))) == waitForExitTask)
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    result = string.IsNullOrWhiteSpace(error) ? output : error;
                }
                else
                {
                    process.Kill(true);
                    result = "Chyba: Vykonávanie kódu trvalo príliš dlho";
                }
            }
            finally
            {
                if (tempPath != null)
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }
                }

                process.Dispose();
            }

            return result;
        }
    }
}
