using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ProductsFunctionApp
{
    public static class GetProductsFunc
    {
        [FunctionName("GetProductsFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            log.LogInformation("Hello from VS.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            List<int> productIds = JsonConvert.DeserializeObject<List<int>>(requestBody);

            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var products = ProductsStore.ProductList.Where(t => productIds.Any(f => f == t.Id));

            return productIds != null
                ? (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(products))
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }


    public static class ProductsStore
    {
        static ProductsStore() {
            PopulateProducts();
        }
        public static List<Product> ProductList { get; private set; }
        private static List<Product> PopulateProducts()
        {
            ProductList = new List<Product>();
            for (int x = 0; x < 20; x++)
            {
                ProductList.Add(new Product()
                {
                    Id = x,
                    Description = "Description " + x,
                    Name = "Name " + x,
                    Price = new Money("USD", 1234 + x)
                });
            }

            return ProductList;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Money Price { get; set; }
    }

    public class Money
    {
        public string Currency { get; }
        public decimal Amount { get; }

        public Money(string currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;
        }
    }
}

