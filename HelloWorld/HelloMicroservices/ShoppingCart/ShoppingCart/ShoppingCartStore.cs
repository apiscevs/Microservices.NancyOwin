namespace ShoppingCart.ShoppingCart
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;

    public class ShoppingCartStore : IShoppingCartStore
    {
        private string connectionString =
    @"Data Source=STONE037\SQLEXPRESS;Initial Catalog=ShoppingCart;
Integrated Security=True";

        private const string readItemsSql =
    @"select ShoppingCartItems.* from ShoppingCart, ShoppingCartItems
where ShoppingCartItems.ShoppingCartId = ShoppingCart.ID
and ShoppingCart.UserId=@UserId"; //TODO: find the way to perform multimapping to nester object (issue is that money class is not populated)

        public async Task<ShoppingCart> Get(int userId)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    var items = await
                      conn.QueryAsync<ShoppingCartItem>(
                        readItemsSql,
                        new { UserId = userId });
                    return new ShoppingCart(userId, items);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private const string deleteAllForShoppingCartSql =
   @"delete item from ShoppingCartItems item
inner join ShoppingCart cart on item.ShoppingCartId = cart.ID
and cart.UserId=@UserId";

        private const string addAllForShoppingCartSql =
  @"insert into ShoppingCartItems 
(ShoppingCartId, ProductCatalogId, ProductName, 
ProductDescription, Amount, Currency)
values 
(@ShoppingCartId, @ProductCatalogId, @ProductName,
@ProductDescription, @Amount, @Currency)";

        private const string CreateShoppingCart = @"INSERT INTO ShoppingCart (UserId) VALUES (@UserId);
SELECT CAST(SCOPE_IDENTITY() as int)";

        public async Task Save(ShoppingCart shoppingCart)
        {
            
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        await conn.ExecuteAsync(
                          deleteAllForShoppingCartSql,
                          new { UserId = shoppingCart.UserId },
                          tx).ConfigureAwait(false);

                        var shoppingCartId = shoppingCart.ID;
                        if (shoppingCartId == 0)
                        {
                            shoppingCartId = await conn.QuerySingleAsync<int>(CreateShoppingCart, new { UserId = shoppingCart.UserId }, tx);
                        }
                        
                        await conn.ExecuteAsync(
                          addAllForShoppingCartSql,
                          shoppingCart.Items.Select(t=> new { 
                              ShoppingCartId = shoppingCartId, 
                              ProductCatalogId = t.ProductCatalogId, 
                              ProductName = t.ProductName,
                              ProductDescription = t.ProductDescription, 
                              Amount = t.Price.Amount, 
                              Currency = t.Price.Currency })
                          ,
                          //shoppingCart.Items,
                          tx).ConfigureAwait(false);
                        tx.Commit();
                    }
                    conn.Close();
                }
            }
            catch(Exception e)
            {

            }
        }

    }
}