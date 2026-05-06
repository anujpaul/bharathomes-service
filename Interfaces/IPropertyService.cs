using bharathome_api.DTOs;

namespace bharathome_api.Interfaces
{
    public interface IPropertyService
    {
        Task<string> CreatePropertyAsync(CreatePropertyDto dto);
    }
}
