using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerceProductCatalog;
using ECommerse.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _repo;
        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product product)
        {
            await _repo.AddProduct(product);
        }

        public  async Task<Product[]> GetAllProductsAsync()
        {
            return (await _repo.GetAllProducts()).ToArray();
        }

        public async Task<Product> GetProductAsync(Guid productId)
        {
            return await _repo.GetProduct(productId);
        }



        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            // return new ServiceReplicaListener[0];

            return new[]
            {
                // Now product catalog is exposing methods to other microservices now
                new ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context,this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repo = new ServiceFabricProductRepository(this.StateManager);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Dell Monitor",
                Description = "Computer Monitor",
                Price = 500,
                Availability = 100
            };
            await _repo.AddProduct(product);
        

            #region Dummy data
            //var product1 = new Product
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Product1",
            //    Description = "Product1 description",
            //    Price = 500,
            //    Availability = 10
            //};
            //var product2 = new Product
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Product2",
            //    Description = "Product2 description",
            //    Price = 600,
            //    Availability = 20
            //};
            //var product3 = new Product
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Product3",
            //    Description = "Product3 description",
            //    Price = 700,
            //    Availability = 30
            //};
            //var product4 = new Product
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Product4",
            //    Description = "Product4 description",
            //    Price = 800,
            //    Availability = 40
            //};

            //await _repo.AddProduct(product1);
            //await _repo.AddProduct(product2);
            //await _repo.AddProduct(product3);
            //await _repo.AddProduct(product4);
            #endregion
            IEnumerable<Product> all = await _repo.GetAllProducts();
        }
    }
}
