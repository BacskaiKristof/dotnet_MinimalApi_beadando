using Microsoft.EntityFrameworkCore;
using ShoppingList;


class ShoppingListDb : DbContext
{
    public ShoppingListDb(DbContextOptions<ShoppingListDb> options)
        : base(options) { }

    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
}