using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public interface IProductCatalogClient
    {
        Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds);
    }
}