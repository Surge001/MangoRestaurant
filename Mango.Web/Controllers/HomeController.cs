using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService productService;
        private readonly ICartService cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            this.productService = productService;
            this.cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            ResponseDto response = await this.productService.GetAllAsync<ResponseDto>("");
            if (response != null)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto products = new();
            ResponseDto response = await this.productService.GetAsync<ResponseDto>(productId, "");
            if (response != null)
            {
                products = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto dto = new CartDto()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto detailsDto = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            ResponseDto response = await this.productService.GetAsync<ResponseDto>(productDto.ProductId, string.Empty);
            if(response != null && response.IsSuccess)
            {
                detailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            List<CartDetailsDto> details = new List<CartDetailsDto>()
            {
                detailsDto
            };
            dto.CartDetails = details;

            // Now Get access token and call Create method on the Cart API:
            string token = await HttpContext.GetTokenAsync("access_token");
            ResponseDto addToCartResponse = await this.cartService.AddToCartAsync<ResponseDto>(dto, token);
            if(addToCartResponse !=null && addToCartResponse.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            string subject = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}