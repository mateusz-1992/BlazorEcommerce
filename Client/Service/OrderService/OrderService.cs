﻿

using Microsoft.AspNetCore.Components;

namespace BlazorEcommerce.Client.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        public OrderService(HttpClient http, IAuthService authService, NavigationManager navigationManager)
        {
            _http = http;
            _authService = authService;
            _navigationManager = navigationManager;
        }

        
        public async Task<string> PlaceOrder()
        {
            if(await _authService.IsUserAuthenticated())
            {
               var result = await _http.PostAsync("api/payment/checkout", null);
                var url = await result.Content.ReadAsStringAsync();
                return url;
            }
            else
            {
                return "login";
            }

        }

        public async Task<List<OrderOverviewResponseDto>> GetOrders()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponseDto>>>("api/order");

            return result.Data;
        }

        public async Task<OrderDetailsResponseDto> GetOrderDetails(int orderId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<OrderDetailsResponseDto>>($"api/order/{orderId}");

            return result.Data;
        }
    }
}
