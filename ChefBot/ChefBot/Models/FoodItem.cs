namespace ChefBot.Models;

public class FoodItem
{
    public string Name { get; set; } = null!;
    public string[] Names { get; set; } = null!;
    public string Emoji { get; set; } = null!;
    public bool IsPlural { get; set; }
}