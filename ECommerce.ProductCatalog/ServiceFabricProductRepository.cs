using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECommerse.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerceProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {

        //We need to use storage. Lets just make the storage work.
        //In order to talk to reliable storage we need to have reference to IReliableStateManager. -This is a built in service fabric interface to work with storage. 

        //The simplest way to store the product is to use something called a Reliable Dictionary - IReliableDictionary. It's similar to normal C# dictionary collection
        // but this maps directly on Service Fabric storage
        
        // In order to get reference to this dictionary I  will write following line of code in line no - 31
        // In this line - I'm trying to get reference to a dictionary called products. if it doesn't exist it will just create it and return me empty reference, hence the name GetOrAdd
        

        // Now all the operations in Service fabric required a transaction, just like working with a database transaction.
        // This is good thing because service fabric commits changes on disk only after transaction is commited.And if error occurs. The whole thing gets reverted.

        // Here I will request a transaction object from the stateManager and make a call to add the product or update it if it already exist. Line no - 39
        // and commit the transaction. Thats it for adding the product. Line no 43
        
        private readonly IReliableStateManager _stateManager;
        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        
        public async Task AddProduct(Product product)
        {
            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);
                await tx.CommitAsync();
            }
        }

        // Moving on to the next method same as before. I will get the dictionary object, create a transaction
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var result = new List<Product>();

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None)) 
                    {

                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                ConditionalValue<Product> product = await products.TryGetValueAsync(tx, productId);

                return product.HasValue ? product.Value : null;
            }
        }
    }
}
