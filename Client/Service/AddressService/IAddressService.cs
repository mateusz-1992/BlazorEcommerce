namespace BlazorEcommerce.Client.Service.AddressService
{
    public interface IAddressService
    {
        Task<Address> GetAddress();
        Task<Address> AddorUpdateAddress(Address address);
    }
}
