using CLDV_POE.Models;
using CLDV_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV_POE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public CustomerController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            return View(customers);
        }

        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.PartitionKey = "CustomerPartition";
                string key = Guid.NewGuid().ToString();
                customer.RowKey = key;
                customer.CustomerId = key;

                await _tableStorageService.AddCustomerAsync(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }
    }
}
