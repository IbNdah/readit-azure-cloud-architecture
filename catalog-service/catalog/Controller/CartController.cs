using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace catalog.Controllers
{
    [ApiController]
    [Route("catalog")]
    public class CatalogController : ControllerBase
    {
        [HttpGet("test-cart")]
        public async Task<IActionResult> TestCart()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("http://cart-service/cart");

            return Ok(response);
        }
    }
}