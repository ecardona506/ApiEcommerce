using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool CategoryExists(int id)
    {
        return _db.Category.Any(c => c.Id == id);
    }

    public bool CategoryExists(string name)
    {
        return _db.Category.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _db.Category.Add(category);
        return Save();
    }

    public bool UpdateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _db.Category.Update(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        _db.Category.Remove(category);
        return Save();
    }

    public ICollection<Category> GetCategories()
    {
        return _db.Category.ToList();
    }

    public Category? GetCategory(int id)
    {
        return _db.Category.FirstOrDefault(c => c.Id == id);
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }
}