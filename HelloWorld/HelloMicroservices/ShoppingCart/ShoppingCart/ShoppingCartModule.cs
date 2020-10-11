using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCartModule : NancyModule
    {
        public ShoppingCartModule(
            IShoppingCartStore shoppingCartStore,
            IProductCatalogClient productCatalogClient//,
           // IEventStore eventStore
            )
            : base("/shoppingcart")
        {
            Get("/{userid:int}", parameters =>
            {
                var userId = (int)parameters.userId;

                var test = productCatalogClient.GetShoppingCartItems(new int[] { 2, 3 }).Result;
                return shoppingCartStore.Get(userId);
            });

            //Post("/{userId:int}/items",
            //    async (parameters, _) =>
            //    {
            //        var productCatalogIds = this.Bind<int[]>();
            //        var userId = (int)parameters.userId;

            //        var shoppingCart = shoppingCartStore.Get(userId);
            //        var shoppingCartItems = await productCatalog
            //            .GetShoppingCartItems(productCatalogIds).ConfigureAwait(false);
            //        shoppingCart.AddItems(shoppingCartItems, eventStore);
            //        shoppingCartStore.Save(shoppingCart);

            //        return shoppingCart;
            //    });

            //Delete("/{userId:int}/items",
            //    parameters =>
            //    {
            //        var productCatalogIds = this.Bind<int[]>();
            //        var userId = (int)parameters.userId;

            //        var shoppingCart = shoppingCartStore.Get(userId);
            //        shoppingCart.RemoveItems(productCatalogIds, eventStore);
            //        shoppingCartStore.Save(shoppingCart);

            //        return shoppingCart;
            //    });
        }
    }
}
