using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MPN.Services.Modeling;
using MPN.Models;

namespace MPN.Pages
{
    public class ParametersPageModel : PageModel
    {
        [BindProperty]
        public ParametersModel param { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            CreateArrayOfParticles(param);
            CreateArrayOfPoints(param);
            CreateArrayOfLayers(param);
            TempData["Message"] = "Моделирование успешно завершено";
            return RedirectToPage();
        }
    }
}
