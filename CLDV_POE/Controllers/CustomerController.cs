using CLDV_POE.Models;
using CLDV_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV_POE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly SqlService _dbContext;

        public CustomerController(TableStorageService tableStorageService, SqlService dbContext)
        {
            _tableStorageService = tableStorageService;
            _dbContext = dbContext;
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

                var customerSql = new CustomerSql
                {
                    CustomerId = customer.CustomerId,
                    Customer_Name = customer.Customer_Name,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                };

                _dbContext.Customers.Add(customerSql);
                await _dbContext.SaveChangesAsync();

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

