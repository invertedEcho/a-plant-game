using Godot;

namespace agame.Items;

public partial class GameItem : GodotObject {
    public string ItemName;
    public string Description;
    public string PathToTexture;
    public int BuyPrice;
    public int SellPrice;

    /// Whether this item is a placeholder. We only have this to be able to fill the inventory array so we can place items at index 0 and index 6 for example.
    /// Maybe there is a better data structure but this works for now
    required public bool IsPlaceHolder;
}

public partial class BuildItem : GameItem {
    public enum BuildItemType {
        GrowPlot
    }

    public static readonly BuildItem GrowPlot = new() {
        Description = "Expand your farm with a new grow plot",
        IsPlaceHolder = false,
        Type = BuildItemType.GrowPlot,
        ItemName = "Grow Plot",
        PathToTexture = "res://assets/preview/grow_plot.png",
        BuyPrice = GameConstants.PlotPrize,
        SellPrice = GameConstants.PlotPrize
    };

    public required BuildItemType Type;
}

public partial class PlantItem : GameItem {
    public required World.GrowPlot.PlantType Type;

    public static readonly PlantItem Cactus = new() {
        Type = World.GrowPlot.PlantType.Cactus,
        BuyPrice = GameConstants.CactusBuyPrize,
        ItemName = "Cactus",
        Description = "This cactus thrives on sunlight and pure attitude. Just don't poke it unless you enjoy regret.",
        PathToTexture = "res://assets/models/nature/cactus/grown_cactus.png",
        SellPrice = GameConstants.CactusSellPrize,
        IsPlaceHolder = false
    };
}