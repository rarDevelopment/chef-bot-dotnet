namespace ChefBot.Models;

public interface IFoodRepository
{
    FoodItem? GetFoodByName(string name);
    FoodItem[]? GetAllFood();
}