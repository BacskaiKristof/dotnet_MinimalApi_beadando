using Microsoft.EntityFrameworkCore;
using ShoppingList;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ShoppingListDb>(opt => opt.UseInMemoryDatabase("ShoppingList"));
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var shoppingListItems = app.MapGroup("/shoppinglistitems");

shoppingListItems.MapGet("/", GetAllShoppingListItems);
shoppingListItems.MapGet("/complete", GetCompleteShoppingListItems);
shoppingListItems.MapGet("/{id}", GetShoppingListItem);
shoppingListItems.MapPost("/", CreateShoppingListItem);
shoppingListItems.MapPut("/{id}", UpdateShoppingListItem);
shoppingListItems.MapPatch("/{id}", PatchShoppingListItem);
shoppingListItems.MapDelete("/{id}", DeleteShoppingListItem);

app.Run();

static async Task<IResult> GetAllShoppingListItems(ShoppingListDb db)
{
    return TypedResults.Ok(await db.ShoppingListItems.Select(x => new ShoppingListItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteShoppingListItems(ShoppingListDb db)
{
    return TypedResults.Ok(await db.ShoppingListItems.Where(t => t.IsComplete).Select(x => new ShoppingListItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetShoppingListItem(int id, ShoppingListDb db)
{
    return await db.ShoppingListItems.FindAsync(id)
        is ShoppingListItem shoppinglistitem
            ? TypedResults.Ok(new ShoppingListItemDTO(shoppinglistitem))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateShoppingListItem(ShoppingListItemDTO shoppingListItemDTO, ShoppingListDb db)
{
    var shoppingListItem = new ShoppingListItem
    {
        IsComplete = shoppingListItemDTO.IsComplete,
        Name = shoppingListItemDTO.Name
    };

    db.ShoppingListItems.Add(shoppingListItem);
    await db.SaveChangesAsync();

    shoppingListItemDTO = new ShoppingListItemDTO(shoppingListItem);

    return TypedResults.Created($"/shoppinglistitems/{shoppingListItem.Id}", shoppingListItemDTO);
}

static async Task<IResult> UpdateShoppingListItem(int id, ShoppingListItemDTO shoppingListItemDTO, ShoppingListDb db)
{
    var shoppinglistitem = await db.ShoppingListItems.FindAsync(id);

    if (shoppinglistitem is null) return TypedResults.NotFound();

    shoppinglistitem.Name = shoppingListItemDTO.Name;
    shoppinglistitem.IsComplete = shoppingListItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> PatchShoppingListItem(int id, ShoppingListItemPatchDTO inputShoppingListItem, ShoppingListDb db)
{
    var shoppinglistitem = await db.ShoppingListItems.FindAsync(id);

    if (shoppinglistitem is null) return TypedResults.NotFound();

    if (inputShoppingListItem.Name is not null) shoppinglistitem.Name = inputShoppingListItem.Name;
    if (inputShoppingListItem.IsComplete is not null) shoppinglistitem.IsComplete = inputShoppingListItem.IsComplete.Value;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteShoppingListItem(int id, ShoppingListDb db)
{
    if (await db.ShoppingListItems.FindAsync(id) is ShoppingListItem shoppinglistitem)
    {
        db.ShoppingListItems.Remove(shoppinglistitem);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}