using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.WebApplication.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
            Message = "Your contact page.";
        }
    }
}
