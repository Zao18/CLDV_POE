using CLDV_POE.Models;
using CLDV_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV_POE.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;
        public OrderController(TableStorageService tableStorageService, QueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrderAsync();
            return View(orders);
        }
        public async Task<IActionResult> Register()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            if (customers == null || customers.Count == 0)
            {
                ModelState.AddModelError("", "No Customers found. Please add Customers first.");
                return View();
            }
            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError("", "No Products found. Please add products first");
                return View();
            }
            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);
                order.PartitionKey = "SightingPartition";
                string key = Guid.NewGuid().ToString();
                order.RowKey = key;
                order.Order_Id = key;
                await _tableStorageService.AddOrderAsync(order);
                string message = $"Oder by customer {order.Order_Id}" + $"if product {order.Product_ID}" + $" on {order.Order_Date}";
                await _queueService.SendMessage(message);
                return RedirectToAction("Index");
            }
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }
    }
}
