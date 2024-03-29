﻿using System.Text.Json;
using ChefBot.Models;

namespace ChefBot;

public class FoodRepository : IFoodRepository
{
    public FoodItem? GetFoodByName(string name)
    {
        return GetAllFood()?.FirstOrDefault(f => f.Names.Contains(name.ToLower().Trim()));
    }

    public FoodItem[]? GetAllFood()
    {
        string filePath = Path.GetFullPath("food.json");
        using StreamReader r = new StreamReader(filePath);
        string json = r.ReadToEnd();
        var food = JsonSerializer.Deserialize<FoodData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return food?.FoodItems;
    }
}