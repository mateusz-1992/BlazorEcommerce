using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Client.Service.ProductService
{
    public interface IProductService
    {
        event Action ProductChanged;
        List<Product> Products { get; set; }
        Task GetProducts(string? categoryUrl = null);
        Task <ServiceResponse<Product>> GetProduct(int productId);
    }
}
