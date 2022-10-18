using ChefBot.Models;

namespace ChefBot.Commands;
public class SeeMenuCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IFoodMessageGenerator _foodMessageGenerator;

    public SeeMenuCommand(IFoodRepository foodRepository, IFoodMessageGenerator foodMessageGenerator)
    {
        _foodRepository = foodRepository;
        _foodMessageGenerator = foodMessageGenerator;
    }

    [SlashCommand("menu", "Show the available food options from Chef Bot.")]
    public async Task SeeMenu()
    {
        var allFood = _foodRepository.GetAllFood();
        if (allFood == null)
        {
            await RespondAsync("Sorry, all of our menus must be on tables at the moment. Try again later!");
            return;
        }
        var sortedFoodStrings = allFood.OrderBy(f => f.Name).Select(f => $"{_foodMessageGenerator.FormatTitleCase(f.Name)} {f.Emoji}");
        var dailySpecial = GetDailySpecial(allFood);
        await RespondAsync($"Today we're serving:\n{string.Join("\n", sortedFoodStrings)}\n\nOur daily special: {_foodMessageGenerator.FormatTitleCase(dailySpecial.Name)} {dailySpecial.Emoji}");
    }

    private FoodItem GetDailySpecial(FoodItem[] allFood)
    {
        var currentDate = DateTime.Now;
        var dateBasedNumber = currentDate.Year + currentDate.Month + currentDate.Day;
        while (dateBasedNumber > allFood.Length)
        {
            dateBasedNumber /= 10;
        }
        return allFood[dateBasedNumber];
    }
}
