using CLDV_POE.Models;
using CLDV_POE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CLDV_POE.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;
        private readonly SqlService _dbContext;

        public OrderController(TableStorageService tableStorageService, QueueService queueService, SqlService dbContext)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
            _dbContext = dbContext;
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
                try
                {
                    order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);
                    order.PartitionKey = "OrderPartition";
                    string key = Guid.NewGuid().ToString();
                    order.RowKey = key;
                    order.Order_Id = key;

                    await _tableStorageService.AddOrderAsync(order);

                    var orderSql = new OrderSql
                    {
                        Order_Id = order.Order_Id,
                        Customer_Id = order.Customer_ID,
                        Product_Id = order.Product_ID,
                        Order_Date = order.Order_Date
                    };

                    _dbContext.Orders.Add(orderSql);
                    await _dbContext.SaveChangesAsync();

                    string message = $"Order by customer {order.Customer_ID} for product {order.Product_ID} on {order.Order_Date}";
                    await _queueService.SendMessage(message);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
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

            // Delete from Azure SQL Database
            var orderSql = await _dbContext.Orders.FindAsync(rowKey);
            if (orderSql != null)
            {
                _dbContext.Orders.Remove(orderSql);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

