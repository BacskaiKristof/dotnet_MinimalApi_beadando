namespace ShoppingList
{
    public class ShoppingListItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }


        public ShoppingListItemDTO() { }


        public ShoppingListItemDTO(ShoppingListItem shoppingListItem) =>
    (Id, Name, IsComplete) = (shoppingListItem.Id, shoppingListItem.Name, shoppingListItem.IsComplete);
    }
}
