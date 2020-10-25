using Nancy;
using Nancy.ModelBinding;
using ShoppingCart.EventFeed;
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
            IProductCatalogClient productCatalogClient,
            IEventStore eventStore
            )
            : base("/shoppingcart")
        {
            Get("/{userid:int}", parameters =>
            {
                var userId = (int)parameters.userId;

                return shoppingCartStore.Get(userId);
            });

            Post("/{userId:int}/items",
                async (parameters, _) =>
                {
                    var productCatalogIds = this.Bind<int[]>();
                    var userId = (int)parameters.userId;

                    var shoppingCart = await shoppingCartStore.Get(userId);
                    var shoppingCartItems = await productCatalogClient
                        .GetShoppingCartItems(productCatalogIds).ConfigureAwait(false);

                    shoppingCart.AddItems(shoppingCartItems, eventStore);
                    await shoppingCartStore.Save(shoppingCart);

                    return shoppingCart;
                });

            Delete("/{userId:int}/items",
                async parameters =>
                {
                    var productCatalogIds = this.Bind<int[]>();
                    var userId = (int)parameters.userId;

                    var shoppingCart = await shoppingCartStore.Get(userId);
                    shoppingCart.RemoveItems(productCatalogIds, eventStore);
                    shoppingCartStore.Save(shoppingCart);

                    return shoppingCart;
                });
        }
    }
}
