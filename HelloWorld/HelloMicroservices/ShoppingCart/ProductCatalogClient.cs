using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private ICache _cache;
        public ProductCatalogClient(ICache cache)
        {
            _cache = cache;
        }

        private static AsyncRetryPolicy exponentialRetryPolicy =
          Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
              20,
              attempt => TimeSpan.FromMilliseconds(1000 * Math.Pow(2, attempt)), (ex, _) => 
              Console.WriteLine(ex.ToString()))
            ;

        private static string productCatalogueBaseUrl =
           // @"https://microservice-getproducts.azurewebsites.net";
        @"http://127.0.0.1:7071";
        private static string getProductPathTemplate =
        // "api/GetProductsFunc?code=DK9kxPuw5rW54iwme9q2E4ve7Z5Tb/wTe0uKLfnaOliQDYqZgQDAUA==";
        "products?productIds=";




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

        private void AddToCache(string resource, HttpResponseMessage response)
        {
            var cacheHeader = response
            .Headers
            .FirstOrDefault(h => h.Key.ToLower() == "cache-control");
            if (string.IsNullOrEmpty(cacheHeader.Key))
                return;
            var maxAge =
            CacheControlHeaderValue.Parse(cacheHeader.Value.FirstOrDefault())
            .MaxAge;
            if (maxAge.HasValue)
                this._cache.Add(key: resource, value: response, ttl: maxAge.Value);
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalogue(int[] productCatalogueIds)
        {
            var productIds = string.Join(",", productCatalogueIds);
            var response = _cache.Get(productIds) as HttpResponseMessage;

            if (response == null)
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(productCatalogueIds), UnicodeEncoding.UTF8, "application/json");
                    httpClient.BaseAddress = new Uri(productCatalogueBaseUrl);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    var query = getProductPathTemplate + productIds;
                    response = await httpClient.GetAsync(query).ConfigureAwait(false);
                    AddToCache(productIds, response);
                }
            }
            return response;
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
            [JsonProperty("productId")]
            public int ProductId { get; set; }
            [JsonProperty("productName")]
            public string ProductName { get; set; }
            [JsonProperty("productDescription")]
            public string ProductDescription { get; set; }
            [JsonProperty("Price")]
            public Money Price { get; set; }
        }
    }
}
