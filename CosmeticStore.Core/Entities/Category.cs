using System.Collections.Generic;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.Entities;

public class Category : BaseEntity, ICategoryComponent
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public int GetTotalProducts()
    {
        int total = Products.Count;
        foreach (var child in Children)
        {
            total += child.GetTotalProducts();
        }
        return total;
    }
}