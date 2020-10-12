using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        
        private static AsyncRetryPolicy exponentialRetryPolicy =
          Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
              20,
              attempt => TimeSpan.FromMilliseconds(1000 * Math.Pow(2, attempt)), (ex, _) => 
              Console.WriteLine(ex.ToString()))
            ;

        private static string productCatalogueBaseUrl =
            @"https://microservice-getproducts.azurewebsites.net";
        //@"http://localhhost:7071";
        private static string getProductPathTemplate =
            "api/GetProductsFunc?code=DK9kxPuw5rW54iwme9q2E4ve7Z5Tb/wTe0uKLfnaOliQDYqZgQDAUA==";
        //"api/GetProductsFunc";
        //https://microservice-getproducts.azurewebsites.net/api/GetProductsFunc?code=DK9kxPuw5rW54iwme9q2E4ve7Z5Tb/wTe0uKLfnaOliQDYqZgQDAUA==


        public Task<IEnumerable<ShoppingCartItem>>
          GetShoppingCartItems(int[] productCatalogueIds) =>
          exponentialRetryPolicy
            .ExecuteAsync(async () => await GetItemsFromCatalogueService(productCatalogueIds).ConfigureAwait(false));

       // public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogueIds) => await GetItemsFromCatalogueService(productCatalogueIds).ConfigureAwait(false);

        private async Task<IEnumerable<ShoppingCartItem>>
          GetItemsFromCatalogueService(int[] productCatalogueIds)
        {
            var response = await
              RequestProductFromProductCatalogue(productCatalogueIds).ConfigureAwait(false);
            return await ConvertToShoppingCartItems(response).ConfigureAwait(false);
        }

        private static async Task<HttpResponseMessage> RequestProductFromProductCatalogue(int[] productCatalogueIds)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(productCatalogueIds), UnicodeEncoding.UTF8, "application/json");
                httpClient.BaseAddress = new Uri(productCatalogueBaseUrl);
                return await httpClient.PostAsync(getProductPathTemplate, content).ConfigureAwait(false);
            }
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var products =
              JsonConvert.DeserializeObject<List<ProductCatalogueProduct>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            return
              products
                .Select(p => new ShoppingCartItem(
                  p.ProductId,
                  p.ProductName,
                  p.ProductDescription,
                  p.Price
              ));
        }

        private class ProductCatalogueProduct
        {
            [JsonProperty("Id")]
            public int ProductId { get; set; }
            [JsonProperty("Name")]
            public string ProductName { get; set; }
            [JsonProperty("Description ")]
            public string ProductDescription { get; set; }
            [JsonProperty("Price")]
            public decimal Price { get; set; }
        }
    }
}
