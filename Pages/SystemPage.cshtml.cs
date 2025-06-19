using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace MPN.Pages
{
    public class SystemPageModel : PageModel
    {
        
        private static string outputHtmlPath = @".\DataFiles\system.html";
        public string HtmlContent { get; set; }
        public void OnGet()
        {
            
                var start = new ProcessStartInfo()
                {
                    FileName = "python.exe",
                    Arguments = @".\PyScripts\draw_system.py",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = Process.Start(start);
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(errors))
                    {
                        throw new Exception($"Ошибка при выполнении скрипта: {errors}");
                    }
                }
                HtmlContent = System.IO.File.ReadAllText(outputHtmlPath);
        }
    }
}
