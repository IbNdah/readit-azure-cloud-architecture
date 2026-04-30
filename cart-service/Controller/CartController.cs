using Microsoft.AspNetCore.Mvc;

namespace cart.Controllers
{
    [ApiController]
    [Route("cart")]
    public class CartController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Cart service is running");
        }
    }
}