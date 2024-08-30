using CLDV_POE.Models;
using CLDV_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV_POE.Controllers
{
    public class ProductController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;

        public ProductController(BlobService blobService, TableStorageService tableStorageService)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                product.ImageUrl = imageUrl;
            }

            if (ModelState.IsValid)
            {
                product.PartitionKey = "ProductPartition";
                string key = Guid.NewGuid().ToString();
                product.RowKey = key;
                product.ProductId = key;
                await _tableStorageService.AddProductAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey, Product product)
        {
            if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                await _blobService.DeleteBlobAsync(product.ImageUrl);
            }
            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }
    }
}
