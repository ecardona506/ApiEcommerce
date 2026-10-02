using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();
    ICollection<Product> GetPaginatedProducts(int pageNumber, int pageSize);
    ICollection<Product> GetProductsByCategory(int categoryId);
    ICollection<Product> SearchProduct(string name);
    Product? GetProduct(int id);
    int GetTotalProducts();
    bool BuyProduct(string name, int quantity);
    bool ProductExists(int id);
    bool ProductExists(string name);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}