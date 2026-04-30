using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContosoUniversity.WebApplication.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory client;

        public IndexModel(IHttpClientFactory client)
        {
            this.client = client;
        }

        public Models.APIViewModels.DepartmentResult Department { get; set; }

        public async Task OnGetAsync()
        {
            var response = await client.CreateClient("client").GetStringAsync("api/Departments");
            Department = JsonSerializer.Deserialize<Models.APIViewModels.DepartmentResult>(response);
        }
    }
}