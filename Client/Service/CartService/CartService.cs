
using BlazorEcommerce.Client.Pages;
using BlazorEcommerce.Shared;
using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CartService(ILocalStorageService localStorageService, HttpClient http, AuthenticationStateProvider authStateProvider)
        {
            _localStorageService = localStorageService;
            _http = http;
            _authStateProvider = authStateProvider;
        }
        public event Action OnChange;

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId
                                        && x.ProductTypeId == cartItem.ProductTypeId);
                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }


                await _localStorageService.SetItemAsync("cart", cart);
            }
            await GetCartItemsCount();
        }

        public async Task<List<CartProductResponseDto>> GetCartProducts()
        {
            if (await IsUserAuthenticated())
            {
                var resposne = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponseDto>>>("api/cart");
                return resposne.Data;
            }
            else
            {
                var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if(cartItems == null)
                    return new List<CartProductResponseDto>();
                var resposne = await _http.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts =
                    await resposne.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponseDto>>>();

                return cartProducts.Data;
            }
            
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if (await IsUserAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId
                        && x.ProductTypeId == productTypeId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorageService.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task StoreCartItems(bool emptylocalcart = false)
        {
            var localCart =  await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if (localCart == null)
            {
                return;
            }

            await _http.PostAsJsonAsync("api/cart", localCart);

            if(emptylocalcart)
            {
                await _localStorageService.RemoveItemAsync("cart");
            }
        }

        public async  Task UpdateQuantity(CartProductResponseDto product)
        {
            if(await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _http.PutAsJsonAsync("/api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId
                        && x.ProductTypeId == product.ProductTypeId);
                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorageService.SetItemAsync("cart", cart);
                }
            }

            
        }

        public async Task GetCartItemsCount()
        {
            if(await IsUserAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorageService.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                await _localStorageService.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);

            }

            OnChange.Invoke();
        }
    }
}
