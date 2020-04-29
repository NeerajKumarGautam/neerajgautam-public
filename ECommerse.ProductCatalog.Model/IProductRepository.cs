using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ECommerse.ProductCatalog.Model
{
   public interface IProductRepository
    {
        // Both methods are asynchronous and returns task
        Task<IEnumerable<Product>> GetAllProducts();
        Task AddProduct(Product product);
        Task<Product> GetProduct(Guid productId);
    }
}
