using ChefBot.Models;

namespace ChefBot.Commands;
public class OrderFoodCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IFoodRepository _foodRepository;
    private readonly IFoodMessageGenerator _foodMessageGenerator;

    public OrderFoodCommand(IFoodRepository foodRepository, IFoodMessageGenerator foodMessageGenerator)
    {
        _foodRepository = foodRepository;
        _foodMessageGenerator = foodMessageGenerator;
    }

    [SlashCommand("order", "Order food or a drink.")]
    public async Task MakeFood(
        [Summary("food_or_drink", "The food you'd like to have")] string foodOrDrink,
        [Summary("user_to_treat", "Specify a user here if you'd like Chef Bot to make food for them instead of yourself")] SocketUser? userToTreat = null
        )
    {
        userToTreat ??= Context.User;

        var food = _foodRepository.GetFoodByName(foodOrDrink);
        if (food == null)
        {
            await RespondAsync(
                $"Sorry, we don't have any \"{foodOrDrink}\" today. I'll have a word with my supplier!");
            return;
        }

        var message = _foodMessageGenerator.Generate(food.Name, userToTreat.Id, food.Emoji, food.IsPlural);
        await RespondAsync(message);
    }
}
