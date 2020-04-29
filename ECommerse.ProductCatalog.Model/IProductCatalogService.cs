using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace ECommerse.ProductCatalog.Model
{
   public  interface IProductCatalogService :IService
    {
        Task<Product[]> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task<Product> GetProductAsync(Guid productId);
    }
}
