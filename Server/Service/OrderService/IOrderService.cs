﻿namespace BlazorEcommerce.Server.Service.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<bool>> PlaceOrder();
        Task<ServiceResponse<List<OrderOverviewResponseDto>>> GetOrders();
        Task<ServiceResponse<OrderDetailsResponseDto>> GetOrderDetails(int orderId);
    }
}
