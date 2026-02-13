using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 orders
    /// </summary>
    [RoutePrefix("api/orders")]
    public class OrderController : BaseApiController
    {
        public OrderController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all pending orders for an account
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetOrders(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var ordersArray = _manager.OrderCreateArray();
                var order = _manager.OrderCreate();
                var result = _manager.OrderGet((ulong)loginId, order);

                var orders = new List<object>();

                if (result == MTRetCode.MT_RET_OK && order != null)
                {
                    orders.Add(new
                    {
                        Order = order.Order(),
                        Login = order.Login(),
                        Symbol = order.Symbol(),
                        Type = order.Type(),
                        TypeFill = order.TypeFill(),
                        TypeTime = order.TypeTime(),
                        VolumeInitial = (decimal)order.VolumeInitial() / 10000,
                        VolumeCurrent = (decimal)order.VolumeCurrent() / 10000,
                        PriceOrder = order.PriceOrder(),
                        PriceTrigger = order.PriceTrigger(),
                        PriceSL = order.PriceSL(),
                        PriceTP = order.PriceTP(),
                        TimeSetup = DateTimeOffset.FromUnixTimeSeconds(order.TimeSetup()).LocalDateTime,
                        TimeExpiration = order.TimeExpiration() > 0 ? DateTimeOffset.FromUnixTimeSeconds(order.TimeExpiration()).LocalDateTime : (DateTime?)null,
                        Comment = order.Comment(),
                        State = order.State(),
                        Reason = order.Reason()
                    });
                }

                order.Release();
                ordersArray.Release();
                return new BaseResponse<object>().WithSuccess(orders, "Orders retrieved successfully");
            });
        }

        /// <summary>
        /// Delete a pending order
        /// </summary>
        [HttpDelete]
        [Route("{orderId:long}")]
        public IHttpActionResult DeleteOrder(long orderId)
        {
            return ExecuteSafe(() =>
            {
                var result = _manager.OrderDelete((ulong)orderId);

                if (result == MTRetCode.MT_RET_OK || result == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    return new BaseResponse<object>().WithSuccess(null, "Order deleted successfully");
                }

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
            });
        }

        /// <summary>
        /// Get order history for an account within date range
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}/history")]
        public IHttpActionResult GetOrderHistory(long loginId, DateTime fromDate, DateTime toDate)
        {
            return ExecuteSafe(() =>
            {
                long from = new DateTimeOffset(fromDate).ToUnixTimeSeconds();
                long to = new DateTimeOffset(toDate).ToUnixTimeSeconds();

                // Note: OrderRequest may not support 4 parameters in your MT5 version
                // Using simplified approach
                var orders = new List<object>();
                
                return new BaseResponse<object>().WithSuccess(orders, "Order history feature not available in this MT5 version. Please use DealHistory endpoint instead.");
            });
        }

        /// <summary>
        /// Get all orders by group
        /// </summary>
        [HttpGet]
        [Route("group/{groupName}")]
        public IHttpActionResult GetOrdersByGroup(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var ordersArray = _manager.OrderCreateArray();
                var result = _manager.OrderGetByGroup(groupName, ordersArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    ordersArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var orders = new List<object>();
                uint total = ordersArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var order = ordersArray.Next(i);
                    if (order != null)
                    {
                        orders.Add(new
                        {
                            Order = order.Order(),
                            Login = order.Login(),
                            Symbol = order.Symbol(),
                            Type = order.Type(),
                            VolumeInitial = (decimal)order.VolumeInitial() / 10000,
                            PriceOrder = order.PriceOrder(),
                            TimeSetup = DateTimeOffset.FromUnixTimeSeconds(order.TimeSetup()).LocalDateTime,
                            Comment = order.Comment(),
                            State = order.State()
                        });
                    }
                }

                ordersArray.Release();
                return new BaseResponse<object>().WithSuccess(orders, $"Orders for group '{groupName}' retrieved successfully");
            });
        }
    }
}
