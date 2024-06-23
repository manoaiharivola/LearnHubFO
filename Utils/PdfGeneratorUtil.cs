using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace LearnHubFO.Utils
{
    public class PdfGeneratorUtil
    {
        private readonly IWebHostEnvironment _env;

        public PdfGeneratorUtil(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            var wkhtmltopdfPath = Path.Combine(_env.WebRootPath, "wkhtmltopdf", "wkhtmltopdf.exe");

            using (var process = new Process())
            {
                process.StartInfo.FileName = wkhtmltopdfPath;
                process.StartInfo.Arguments = "-q - -";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                using (var streamWriter = process.StandardInput)
                {
                    streamWriter.Write(htmlContent);
                }

                using (var memoryStream = new MemoryStream())
                {
                    process.StandardOutput.BaseStream.CopyTo(memoryStream);
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        var errorMessage = process.StandardError.ReadToEnd();
                        throw new Exception($"wkhtmltopdf failed. Exit code: {process.ExitCode}. Error message: {errorMessage}");
                    }

                    return memoryStream.ToArray();
                }
            }
        }

    }
}
