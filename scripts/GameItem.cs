using Godot;

public partial class GameItem : GodotObject {
    public string DescriptionName;
    public string PathToTexture;
    public int SellPrice;
    public int BuyPrice;
    /// Whether this item is a placeholder. We only have this to be able to fill the inventory array so we can place items at index 0 and index 6 for example.
    /// Maybe there is a better data structure but this works for now
    required public bool IsPlaceHolder;
}
