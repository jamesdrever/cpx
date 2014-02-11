using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Orders
{

    public class OrderRepository : IOrderRepository
    {
        private readonly Database _database;

        public OrderRepository(Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            _database = database;
        }

        public OrderOperationStatus GetOrder(Contact contact)
        {
            return GetOrder(contact, 1);
        }

        public OrderOperationStatus GetOrder(Contact contact, int orderIndex)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                var sql = Sql.Builder
                    .Select("*")
                    .From("cpxOrder");

                sql.Append("WHERE status='PROV' AND OrderIndex=@0",orderIndex);

                if (contact.ContactId > 0)
                {
                    sql.Append("AND ContactId=@0", contact.ContactId);
                }
                else
                    sql.Append("AND UserId=@0", contact.UserId);

                var order =
                    _database.SingleOrDefault<Order>(sql);

                //if no order found with contact id, try user id
                if (contact.ContactId > 0 & order == null)
                {
                    sql = Sql.Builder
                             .Select("*")
                             .From("cpxOrder")
                             .Where("status='PROV' AND OrderIndex=@0 AND UserId=@1",orderIndex,contact.UserId);
                    order = _database.SingleOrDefault<Order>(sql);
                }

                if (order == null)
                {
                    order = CreateOrder(contact, orderIndex);
                }
                if (contact.ContactId != order.ContactId)
                {
                    order.ContactId = contact.ContactId;
                    _database.Update(order);
                }

                order.OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0",order.OrderId);
                operationStatus.Order = order;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred retrieving the order", e);
            }
            return operationStatus;
        }

        public OrdersOperationStatus GetOrders(Contact contact)
        {
            var operationStatus = new OrdersOperationStatus();
            try
            {
                var sql = Sql.Builder
                    .Select("*")
                    .From("cpxOrder")
                    .Where("status='PROV'");

                if (contact.ContactId > 0)
                    sql.Append("AND ContactId=@0", contact.ContactId);
                else
                    sql.Append("AND UserId=@0", contact.UserId);

                var orders =
                    _database.Fetch<Order>(sql);
                
                for (int i=0;i<orders.Count;i++)
                {
                    if (contact.ContactId != orders[i].ContactId)
                    {
                        orders[i].ContactId = contact.ContactId;
                        _database.Update(orders[i]);
                    }
                    orders[i].OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0", orders[i].OrderId);
                }
                operationStatus.Orders = orders;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus.LogFailedOperation(e,"An error has occurred updating the order");
            }
            return operationStatus;
        }

        private Order CreateOrder(Contact contact, int orderIndex)
        {
            var order = new Order {UserId = contact.UserId, UserName = contact.UserName, ContactId=contact.ContactId,OrderIndex = orderIndex,GiftAidAgreement = contact.GiftAidAgreement};
            order.OrderId = (int) _database.Insert("cpxOrder", "OrderId", true, order);
            return order;
        }

        public OrderOperationStatus SaveOrder(Order order)
        {
            return SaveOrder(order, true);
        }


        public OrderOperationStatus SaveOrder(Order order, bool updateOrderLines)
        {
            var orderOperationStatus = new OrderOperationStatus();
            try
            {
                if (!_database.IsNew(order))
                {
                    _database.Update(order);
                    if (updateOrderLines)
                    {
                        for (int i = 0; i < order.OrderLines.Count; i++)
                        {
                            if (_database.IsNew(order.OrderLines[i]))
                            {
                                order.OrderLines[i].OrderLineId =
                                    (int)_database.Insert(order.OrderLines[i]);
                                
                            }
                            else
                            {
                                _database.Update(order.OrderLines[i]);
                            }
                        }
                    }
                    orderOperationStatus.Status = true;
                    orderOperationStatus.Message = "The order has been saved successfully.";
                }
                else
                {
                    orderOperationStatus.Status = false;
                    orderOperationStatus.Message = "The order could not be found.";

                }
                orderOperationStatus.Order = order;

            }
            catch (Exception e)
            {
                orderOperationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred saving the order"+e.Message+e.StackTrace, e);

            }
            return orderOperationStatus;

        }

        /**
        public OrderOperationStatus Add(Product product, int quantity, string paymentType, decimal paymentAmount,
                                        Contact contact)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                if (quantity > product.MaximumQuantity && product.MaximumQuantity != 0)
                    return new OrderOperationStatus
                        {
                            Message = "No more items of this type can be added!",
                            Status = false
                        };

                //get the current order
                operationStatus = GetOrder(contact, product.OrderIndex);
                //check if the product is already in the order
                if (operationStatus.Order.ContainsProduct(product))
                {
                    //if it is, add the new quantity to the existing one.
                    int existingQuantity = operationStatus.Order.GetOrderLine(product).Quantity;
                    return Update(product, product, existingQuantity + quantity, paymentType, paymentAmount, contact);
                }
                var orderLine = new OrderLine
                    {
                        OrderId = operationStatus.Order.OrderId,
                        ProductId = product.ProductId,
                        ProductPrice = product.Price,
                        ProductTitle = product.Title,
                        ProductOptionId = product.OptionId,
                        ProductOptionTitle = product.OptionTitle,
                        ProductCategory = product.Category,
                        ProductCode=product.Code,
                        ProductOptionExternalId = product.OptionExternalId,
                        ProductType = product.ProductType,
                        PaymentType = paymentType,
                        PaymentAmount = paymentAmount,
                        Quantity = quantity
                    };
                orderLine.InjectFrom(product);
                orderLine.OrderLineId = (int) _database.Insert("cpxOrderLines", "orderLineId", true, orderLine);
                //get the updated order
                operationStatus = GetOrder(contact, product.OrderIndex, true);
                operationStatus.OrderLine = orderLine;
                operationStatus.Message = "Added " + product.Title + " successfully!";
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred adding the product to the order", e);
            }
            return operationStatus;

        }

        public OrderOperationStatus Update(Product oldProduct, Product newProduct, int quantity, string paymentType,
                                           decimal paymentAmount, Contact contact)
        {
            // if the quantity is zero, do a remove instead
            if (quantity == 0)
                return Remove(oldProduct, contact);

            if (quantity > newProduct.MaximumQuantity && newProduct.MaximumQuantity != 0)
                return new OrderOperationStatus {Message = "No more items of this type can be added!", Status = false};

            var operationStatus = new OrderOperationStatus();
            try
            {
                //get the order
                operationStatus = GetOrder(contact, oldProduct.OrderIndex);
                if (operationStatus.Status)
                {
                    //check that the product to be updated exists
                    if (!operationStatus.Order.ContainsProduct(oldProduct))
                    {
                        return new OrderOperationStatus
                            {
                                Message = "The item cannot be found in the order, and therefore cannnot be updated",
                                Status = false
                            };
                    }
                    //check that the product to be udated to doesn't exist
                    if (newProduct.OptionId > 0 && oldProduct.OptionId != newProduct.OptionId &&
                        operationStatus.Order.ContainsProduct(newProduct))
                    {
                        return new OrderOperationStatus
                            {
                                Message = "There is already an item of this type in the order",
                                Status = false
                            };
                    }
                    //get the order line to update
                    OrderLine orderLine = operationStatus.Order.GetOrderLine(oldProduct);
                    //merge in changes from the update
                    orderLine.ProductPrice = newProduct.Price;
                    orderLine.ProductTitle = newProduct.Title;
                    orderLine.ProductOptionTitle = newProduct.OptionTitle;
                    orderLine.ProductOptionId = newProduct.OptionId;
                    orderLine.ProductCategory = newProduct.Category;
                    orderLine.ProductOptionExternalId = newProduct.OptionExternalId;
                    orderLine.Quantity = quantity;
                    orderLine.PaymentType = paymentType;
                    orderLine.PaymentAmount = paymentAmount;
                    orderLine.InjectFrom(newProduct);
                    //make the update
                    _database.Update("cpxOrderLines", "OrderLineId", orderLine);
                    //get the updated order
                    operationStatus = GetOrder(contact, newProduct.OrderIndex, true);
                    if (operationStatus.Status)
                    {
                        operationStatus.OrderLine = orderLine;
                        operationStatus.Message = "Updated " + oldProduct.Title + " successfully!";
                    }
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred adding the product to the order", e);
            }
            return operationStatus;
        }

        public OrderOperationStatus Remove(Product product, Contact contact)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                // Create a PetaPoco database object
                var orderResult =
                    _database.Execute(
                        "DELETE cpxOrderLines FROM cpxOrderLines ol INNER JOIN cpxOrder o ON ol.OrderId=o.OrderId WHERE ol.ProductId=@0 AND ol.ProductOptionId=@1 AND o.Status='P' AND (o.UserId=@2 OR o.UserName=@3)",
                        product.ProductId, product.OptionId, contact.UserId, contact.UserName);
                operationStatus = GetOrder(contact, product.OrderIndex, true);
                if (operationStatus.Status)
                {
                    operationStatus.Message = "Removed " + product.Title + " successfully!";
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred removing the product from the order", e);
            }
            return operationStatus;
        }

        public OrderOperationStatus RemoveAll(Contact contact)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                // Create a PetaPoco database object
                var orderResult =
                    _database.Execute(
                        "DELETE cpxOrderLines FROM cpxOrderLines ol INNER JOIN cpxOrder o ON ol.OrderId=o.OrderId WHERE (o.UserId=@0 OR o.UserName=@1)",
                        contact.UserId, contact.UserName);
                operationStatus = GetOrder(contact);
                operationStatus.Message = "Removed all successfully!";
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred removing the product from the order", e);
            }
            return operationStatus;
        }

        

        public OrderOperationStatus UpdateSpecialRequirements(string specialRequirements, int orderIndex,
                                                              Contact contact)
        {
            var operationStatus = new OrderOperationStatus();

            try
            {
                operationStatus = GetOrder(contact, orderIndex);
                if (operationStatus.Status)
                {
                    var order = operationStatus.Order;
                    order.SpecialRequirements = specialRequirements;
                    _database.Update("cpxOrder", "OrderId", order);
                    //TODO: what best to return here?
                    operationStatus.Order = order;
                    operationStatus.Message = "Special Requirements updated successfully!";
                    operationStatus.Status = true;
                }

            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred updating the special requirements", e);
            }
            return operationStatus;
        }
        

        public OrderOperationStatus AddVoucher(Voucher voucher, Contact contact)
        {
            var operationStatus = new OrderOperationStatus();

            try
            {
                operationStatus = GetOrder(contact, voucher.OrderIndex);
                if (operationStatus.Status)
                {
                    var order = operationStatus.Order;
                    order.VoucherId = voucher.VoucherId;
                    order.VoucherAmount = voucher.Amount;
                    order.VoucherPercentage = voucher.Percentage;
                    order.VoucherPerItemAmount = voucher.PerItemAmount;
                    order.VoucherProductCategoryFilter = voucher.ProductCategoryFilter;
                    order.VoucherCategoryFilter = voucher.VoucherCategoryFilter;
                    order.VoucherMinimumItems = voucher.MinimumItems;
                    order.VoucherMinimumPayment = voucher.MinimumPayment;
                    order.VoucherInfo = "Voucher used: " + voucher.Title;
                    _database.Update("cpxOrder", "OrderId", order);
                    operationStatus.Order = order;
                    operationStatus.Message = "Voucher " + voucher.Title + " added successfully!";
                    operationStatus.Status = true;

                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred adding the voucher", e);
            }
            return operationStatus;
        }
        **/

        public OrderOperationStatus GetOrder(int orderId)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                var order =
                    _database.SingleOrDefault<Order>(
                        "SELECT * FROM cpxOrder WHERE OrderId=@0",
                        orderId);
                order.OrderLines = _database.Fetch<OrderLine>("SELECT * FROM cpxOrderLines WHERE OrderId=@0",
                                                              orderId);
                operationStatus.Order = order;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred getting the order", e);
            }
            return operationStatus;


        }

        public OrderOperationStatus ProcessOrder(Order order, Contact contact)
        {
            throw new NotImplementedException();
        }



    /**
                 public CartOperationStatus RefreshCart()
                {
                    return RefreshCart(1);

                }

                public CartOperationStatus RefreshCart(int cartIndex)
                {
                    var opStatus = new CartOperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        Cart cart = GetCart(cartIndex, context);
                        ApplyDiscountsAndShipping(cart);
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                            opStatus.UpdatedCart = cart;
                        }
                        catch (Exception ex)
                        {
                            opStatus = CartOperationStatus.CreateFromException("Error refreshing cart: " + ex.Message, ex);

                        }
                        return opStatus;

                    }
                }* 
         * 
         * 
         * 
                public int GetCartID()
                {
                    return GetCartID(1);
                }

                public int GetCartID(int index)
                {
                    return GetCart(index).CartID;
                }

                public Cart GetCart(string SessionID)
                {
                    return GetCart(SessionID, 1);
                }

                public Cart GetCart(string SessionID, int CartNumber)
                {
                    using (var context = new CartContext())
                    {
                        var Carts = context.Cart.Include("CartItems").Where(t => t.SessionID.Equals(SessionID) && t.CartNumber.Equals(CartNumber) && (!t.Status.Equals("C"))).ToList().FirstOrDefault();
                        return ((Cart)Carts);
                    }
                }

                public List<Cart> GetCartsByUserName(string UserName)
                {
                    using (var context = new CartContext())
                    {
                        var Carts = context.Cart.Include("CartItems").Where(t => t.UserName.Equals(UserName) && t.CartItems.Count > 0).ToList();
                        return Carts;
                    }
                }


                private Cart GetCart(int CartNumber, CartContext context)
                {
                    //get the current session ID
                    var UserId = _contactRepository.GetContactID();
                    //if no session ID, create new cart(s)
                    if (UserId == null)
                    {
                        return CreateCarts(_config.GetConfiguration().NumberOfCarts, context)[CartNumber - 1];
                    }
                    else
                    {
                        //otherwise try to retrieve current cart
                        var Carts = context.Cart.Include("CartItems").Where(t => t.SessionID.Equals(UserId) && t.CartNumber.Equals(CartNumber) && (!t.Status.Equals("C")));
                        if (Carts.Count() > 0) { var Cart = Carts.ToList().FirstOrDefault(); return Cart; }
                        //if (Cart != null)
                        //{
                        //    return Cart;
                        //}
                        //if no curernt 
                        else
                        {
                            return CreateCarts(_config.GetConfiguration().NumberOfCarts, context)[CartNumber - 1];
                        }
                    }
                }

                private List<Cart> CreateCarts(int noofcarts, CartContext context)
                {
                    List<Cart> carts = new List<Cart>();
                    string userName = _contactRepository.GetContactUserName();

                    for (int i = 1; i <= noofcarts; i++)
                    {
                        var cart = new Cart { CartItems = new List<CartItem>(), SessionID = _contactRepository.GetContactID(), UserName = userName, CartNumber = i, Status = "O", Shipping = 0f, Discount = 0f, CartCreated = DateTime.Now };
                        carts.Add(cart);
                        context.Cart.Add(cart);
                    }
                    context.SaveChanges();
                    //}
                    return carts;
                }
                /// <summary>
                /// get the cart using the cart's id number
                /// </summary>
                /// <param name="id"></param>
                /// <returns></returns>
                public Cart GetCartByID(int CartID)
                {
                    using (var context = new CartContext())
                    {
                        return GetCartByID(CartID, context);
                    }
                }

                /// <summary>
                /// get the cart using the cart's id number
                /// </summary>
                /// <param name="CartID">The cart ID.</param>
                /// <param name="context">The context.</param>
                /// <returns></returns>
                private Cart GetCartByID(int CartID, CartContext context)
                {
                    var Carts = context.Cart.Include("CartItems").Where(t => t.CartID.Equals(CartID)).FirstOrDefault();
                    return ((Cart)Carts);
                }

                public CartOperationStatus Add(int umbracoNodeId, string displayId, int quantity, string option, float price)
                {
                    return Add(1, umbracoNodeId, displayId, quantity, option, null, price);
                }

                public CartOperationStatus Add(int umbracoNodeId, string displayId, int quantity, string option, string itemtype, float price)
                {
                    return Add(1, umbracoNodeId, displayId, quantity, option, itemtype, price);
                }

                public CartOperationStatus Add(int umbracoNodeId, string displayId, int quantity, string option, float price, int cartIndex)
                {
                    return Add(cartIndex, umbracoNodeId, displayId, quantity, option, null, price);
                }

                public CartOperationStatus Add(int umbracoNodeId, string displayId, int quantity, string option, string itemtype, float price, int cartIndex)
                {
                    return Add(cartIndex, umbracoNodeId, displayId, quantity, option, itemtype, price);
                }

                public CartOperationStatus Add(int cartNumber, int productId, string title, int quantity, string option, string itemtype, float price)
                {
                    var opStatus = new CartOperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartNumber, context);
                        CartItem existingCartItem = null;
                        if (cart.CartItems != null)
                        {
                            existingCartItem = cart.CartItems.Where(item => item.ProductId == productId).FirstOrDefault();
                        }
                        if (existingCartItem == null)
                        {
                            cart.CartItems.Add(new CartItem
                            {
                                Option = option,
                                Title = title,
                                ProductId = productId,
                                Price = price,
                                Quantity = quantity,
                                ItemType = itemtype
                            });
                            //context.Entry(newItem).State = System.Data.EntityState.Modified;
                        }
                        else
                        {
                            existingCartItem.Quantity += quantity;
                            existingCartItem.Price = price;
                        }
                        ApplyDiscountsAndShipping(cart);
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                            opStatus.UpdatedCart = cart;
                        }
                        catch (Exception ex)
                        {
                            opStatus = CartOperationStatus.CreateFromException("Error adding item to basket: " + ex.Message, ex);

                        }
                        return opStatus;
                    }
                }


                public CartOperationStatus Remove(int umbracoNodeId)
                {
                    return Remove(1, umbracoNodeId);
                }

                public CartOperationStatus Remove(int cartIndex, int productId)
                {
                    var opStatus = new CartOperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        var cartItem = cart.CartItems.Where(item => item.ProductId == productId).FirstOrDefault();
                        ((IList<CartItem>)cart.CartItems).Remove(cartItem);
                        ApplyDiscountsAndShipping(cart);
                        context.Entry(cartItem).State = System.Data.EntityState.Deleted;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                            opStatus.UpdatedCart = cart;
                        }
                        catch (Exception ex)
                        {
                            opStatus = CartOperationStatus.CreateFromException("Error removing item from basket: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }

                public CartOperationStatus Update(int id, int quantity)
                {
                    return Update(1, id, quantity);
                }

                public CartOperationStatus Update(int cartIndex, int productId, int quantity)
                {
                    var opStatus = new CartOperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        var cartItem = cart.CartItems.Where(item => item.ProductId == productId).FirstOrDefault();

                        cartItem.Quantity = quantity;
                        var index = ((IList<CartItem>)cart.CartItems).IndexOf(cartItem);
                        cart.CartItems[index] = cartItem;
                        ApplyDiscountsAndShipping(cart);
                        context.Entry(cartItem).State = System.Data.EntityState.Modified;

                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                            opStatus.UpdatedCart = cart;
                        }
                        catch (Exception ex)
                        {
                            opStatus = CartOperationStatus.CreateFromException("Error updating item in basket: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }

                public CartOperationStatus Update(int productId, int quantity, string itemtype, float price)
                {
                    return Update(1, productId, quantity, itemtype, price);
                }




                public CartOperationStatus Update(int cartIndex, int productId, int quantity, string itemtype, float price)
                {
                    var opStatus = new CartOperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        var cartItem = cart.CartItems.Where(item => item.ProductId == productId).FirstOrDefault();

                        cartItem.Quantity = quantity;
                        cartItem.Price = price;
                        cartItem.ItemType = itemtype;
                        var index = ((IList<CartItem>)cart.CartItems).IndexOf(cartItem);
                        cart.CartItems[index] = cartItem;
                        ApplyDiscountsAndShipping(cart);
                        context.Entry(cartItem).State = System.Data.EntityState.Modified;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                            opStatus.UpdatedCart = cart;
                        }
                        catch (Exception ex)
                        {
                            opStatus = CartOperationStatus.CreateFromException("Error updating item in basket: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }

                public OperationStatus UpdateSpecialRequirements(string SpecialRequirements)
                {
                    return UpdateSpecialRequirements(0, SpecialRequirements);
                }

                public OperationStatus UpdateSpecialRequirements(int cartIndex, string SpecialRequirements)
                {
                    var opStatus = new OperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        cart.SpecialRequirements = SpecialRequirements;
                        context.Entry(cart).State = System.Data.EntityState.Modified;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                        }
                        catch (Exception ex)
                        {
                            opStatus = OperationStatus.CreateFromException("Error updating special requirements: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }



                public OperationStatus UpdateStatus(string status)
                {
                    return UpdateStatus(1, status);
                }


                public OperationStatus UpdateStatus(int cartIndex, string status)
                {
                    var opStatus = new OperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        cart.Status = status;
                        context.Entry(cart).State = System.Data.EntityState.Modified;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                        }
                        catch (Exception ex)
                        {
                            opStatus = OperationStatus.CreateFromException("Error updating status: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }
                public OperationStatus UpdateStatusByCartID(int cartID, string status)
                {
                    var opStatus = new OperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCartByID(cartID, context);
                        cart.Status = status;
                        context.Entry(cart).State = System.Data.EntityState.Modified;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                        }
                        catch (Exception ex)
                        {
                            opStatus = OperationStatus.CreateFromException("Error updating status: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }

                public OperationStatus UpdateUserName(string UserName)
                {
                    return UpdateUserName(1, UserName);
                }

                public OperationStatus UpdateUserName(int cartIndex, string UserName)
                {
                    var opStatus = new OperationStatus { Status = true };
                    using (var context = new CartContext())
                    {
                        var cart = GetCart(cartIndex, context);
                        cart.UserName = UserName;
                        context.Entry(cart).State = System.Data.EntityState.Modified;
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                        }
                        catch (Exception ex)
                        {
                            opStatus = OperationStatus.CreateFromException("Error updating username: " + ex.Message, ex);
                        }
                        return opStatus;
                    }
                }
                /// <summary>
                /// updates the username for all current cards
                /// intended for use when the user logins in and all
                /// carts need to be updated with their username (rather than just
                /// the session id)
                /// </summary>
                /// <param name="UserName"></param>
                /// <returns></returns>
                public OperationStatus UpdateUserNameForAllCarts(string UserName)
                {
                    var opStatus = new OperationStatus { Status = true };
                    //get the current session ID
                    var UserId = _contactRepository.GetContactID();
                    //if there is session ID,)
                    if (UserId == null)
                    {
                        opStatus.Message = "No carts to update";
                        return opStatus;
                    }

                    using (var context = new CartContext())
                    {
                        var Carts = context.Cart.Include("CartItems").Where(t => t.SessionID.Equals(UserId) && (!t.Status.Equals("C")));
                        foreach (var cart in Carts)
                        {
                            cart.UserName = UserName;
                            context.Entry(cart).State = System.Data.EntityState.Modified;
                        }
                        try
                        {
                            opStatus.Status = context.SaveChanges() > 0;
                        }
                        catch (Exception ex)
                        {
                            opStatus = OperationStatus.CreateFromException("Error updating username: " + ex.Message, ex);
                        }

                    }
                    return opStatus;
                }

                public string GetUserName()
                {
                    return GetUserName(1);
                }


                public string GetUserName(int cartIndex)
                {
                    return GetCart(cartIndex).UserName;
                }

                private void ApplyDiscountsAndShipping(Cart cart)
                {
                    cart.Discount = (float)_discountHandlerFactory.getDiscountHandler(_config.GetConfiguration().GetDiscountHandler(cart.CartNumber)).getDiscount(cart);
                    cart.Shipping = (float)_shippingHandlerFactory.getShippingHandler(_config.GetConfiguration().GetShippingHandler(cart.CartNumber)).getShipping(cart, _contactRepository.GetDeliveryCountry());
                }
         **/












    }
}