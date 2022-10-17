using System.Text.RegularExpressions;

namespace ChefBot
{
    public class FoodMessageGenerator : IFoodMessageGenerator
    {
        private const int EatThreshold = 15;
        private const int MaxForOdds = 100;

        private const string FoodKeyword = "{{FOOD}}";
        private const string PersonKeyword = "{{PERSON}}";
        private const string EmojiKeyword = "{{EMOJI}}";

        private readonly string[] _messageTextOptions;

        public FoodMessageGenerator()
        {
            _messageTextOptions = new[] {
                $"{EmojiKeyword} Made {FoodKeyword} for {PersonKeyword}",
                $"{EmojiKeyword} Serving up some {FoodKeyword} for {PersonKeyword}",
                $"{EmojiKeyword} Hope you like {FoodKeyword}, {PersonKeyword}",
                $"{EmojiKeyword} {FoodKeyword} eh? Very well, {PersonKeyword}, I've made some",
                $"{EmojiKeyword} {FoodKeyword} coming right up, {PersonKeyword}",
                $"{EmojiKeyword} {FoodKeyword} fresh out of the kitchen for {PersonKeyword}"
            };
        }

        public string Generate(string food, ulong userId, string emoji, bool isPlural)
        {
            var randomMessage = GetRandomFoodMessage();
            var foodMessage = ReplaceKeywords(randomMessage, FormatTitleCase(food.ToLower()), $"<@{userId}>", emoji);

            if (CalculateDidEat())
            {
                foodMessage += $"...but, sorry, I ate {(isPlural ? "them" : "it")} 😳";
            }
            else
            {
                foodMessage += $", please enjoy 😋";
            }
            return foodMessage;
        }

        private string FormatTitleCase(string text)
        {
            var regex = new Regex("^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$");
            var splitFormatted = text
                .ToLower()
                .Split(" ")
                .Select(s => regex.Match(s).Success ? s.ToUpper() : s[0].ToString().ToUpper() + s[1..]);
            var formattedTitle = string.Join(" ", splitFormatted);
            return formattedTitle;
        }

        private string ReplaceKeywords(string text, string foodName, string personName, string emoji)
        {
            return text
                .Replace(FoodKeyword, foodName)
                .Replace(PersonKeyword, personName)
                .Replace(EmojiKeyword, emoji);
        }

        private string GetRandomFoodMessage()
        {
            var random = new Random();
            int arrayIndex = random.Next(_messageTextOptions.Length);
            return _messageTextOptions[arrayIndex];
        }

        private bool CalculateDidEat()
        {
            var random = new Random();
            int number = random.Next(MaxForOdds);
            return number < EatThreshold;
        }
    }

    public interface IFoodMessageGenerator
    {
        string Generate(string food, ulong userId, string emoji, bool isPlural);
    }
}
