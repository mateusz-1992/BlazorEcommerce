namespace BlazorEcommerce.Client.Service.OrderService
{
    public interface IOrderService
    {
        Task PlaceOrder();
        Task<List<OrderOverviewResponseDto>> GetOrders();
        Task<OrderDetailsResponseDto> GetOrderDetails(int orderId);
    }
}
