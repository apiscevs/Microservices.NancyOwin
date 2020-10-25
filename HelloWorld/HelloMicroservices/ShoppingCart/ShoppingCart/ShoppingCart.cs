namespace ShoppingCart.ShoppingCart
{
    using global::ShoppingCart.EventFeed;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    //using EventFeed;

    public class ShoppingCart
    {
        private HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();

        public int ID { get; set; }
        public int UserId { get; }
        public IEnumerable<ShoppingCartItem> Items { get { return items; } }

        public ShoppingCart(int userId)
        {
            this.UserId = userId;
        }

        public ShoppingCart(int userId, IEnumerable<ShoppingCartItem> items)
        {
            this.UserId = userId;
            foreach (var item in items)
            {
                this.items.Add(item);
            }
        }


        public void AddItems(
          IEnumerable<ShoppingCartItem> shoppingCartItems,
          IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
                if (this.items.Add(item))
                    eventStore.Raise(
                      "ShoppingCartItemAdded",
                      new { UserId, item });
        }

        public void RemoveItems(
          long[] productCatalogueIds,
          IEventStore eventStore)
        {
            items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogId));
        }
    }

    public class ShoppingCartItem
    {
        public int ShoppingCartId { get; }
        public long ProductCatalogId { get; }
        public string ProductName { get; }
        public string ProductDescription { get; }
        public Money Price { get; set; }

        public ShoppingCartItem(
          int productCatalogueId,
          string productName,
          string description,
          Money price)
        {
            this.ProductCatalogId = productCatalogueId;
            this.ProductName = productName;
            this.ProductDescription = description;
            this.Price = price;
        }

        //for dapper
        public ShoppingCartItem(System.Int32 ID, System.Int32 ShoppingCartId, System.Int64 ProductCatalogId, System.String ProductName, System.String ProductDescription, System.Int32 Amount, System.String Currency)
        {
            this.ProductCatalogId = ProductCatalogId;
            this.ProductName = ProductName;
            this.ProductDescription = ProductDescription;
            this.Price = new Money(Currency, Amount);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = obj as ShoppingCartItem;
            return this.ProductCatalogId.Equals(that.ProductCatalogId);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.ProductCatalogId.GetHashCode();
        }
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