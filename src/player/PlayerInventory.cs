using agame.Items;
using Godot;
using Godot.Collections;
using static agame.Items.BuildItem;
using static agame.World.GrowPlot;

namespace agame.Player;

public partial class PlayerInventory : Node {
    public const int HotbarSize = 8;
    public Array<GameItem> Hotbar = [];

    private Player _player;

    public int CurrentHotbarSlotSelected = 0;


    public override void _Ready() {
        _player = GetParent<Player>();
        CreateInitialHotbarItems();
    }

    private void CreateInitialHotbarItems() {
        for (int i = 0; i < HotbarSize; i++) {
            GD.Print("adding default item to hotbar");
            Hotbar.Add(GrowPlot);
        }
    }

    /// <summary>
    /// Attempts to place the given item into the currently selected hotbar slot.
    /// If the selected slot is occupied, tries to place the item into the next
    /// available empty slot from left to right.
    /// </summary>
    /// <param name="itemToAdd">The item to add to the hotbar.</param>
    /// <returns>
    /// True if the item is placed successfully; otherwise false.
    /// </returns>
    public bool TryPlaceItemInHotbar(GameItem itemToAdd) {
        if (Hotbar[CurrentHotbarSlotSelected].IsPlaceHolder) {
            Hotbar[CurrentHotbarSlotSelected] = itemToAdd;
            UiManager.Instance.UpdateItemPreviewSlotTexture(CurrentHotbarSlotSelected, itemToAdd.PathToTexture);
            return true;
        }
        for (int index = 0; index < Hotbar.Count; index++) {
            GameItem currentGameItem = Hotbar[index];

            if (!currentGameItem.IsPlaceHolder) continue;

            Hotbar[index] = itemToAdd;
            UiManager.Instance.UpdateItemPreviewSlotTexture(index, itemToAdd.PathToTexture);
            return true;
        }
        return false;
    }

    /// Returns a boolean indicating whether the removal of the item at the given index was succesful
    public bool RemoveItemFromHotbar(int index) {
        if (index + 1 > HotbarSize) {
            GD.PrintErr($"The inventory has a size of {HotbarSize} so the given index {index} is invalid");
            return false;
        }

        Hotbar[index] = new GameItem { IsPlaceHolder = true };
        UiManager.Instance.UpdateItemPreviewSlotTexture(index, null);
        return true;
    }

    /// Returns the game item and index for the given GameItemType
    public (PlantItem, int)? GetPlantItemByType(PlantType plantType) {
        for (int index = 0; index < HotbarSize; index++) {
            GameItem currentItem = Hotbar[index];
            if (currentItem is PlantItem plantItem && plantItem.Type == plantType) {
                return (plantItem, index);
            }
        }
        return null;
    }

    public int GetPlayerOwnCountForPlantItemByType(PlantType plantType) {
        int count = 0;
        for (int index = 0; index < HotbarSize; index++) {
            GameItem currentItem = Hotbar[index];
            if (currentItem is PlantItem plantItem && plantItem.Type == plantType) {
                count++;
            }
        }
        return count;
    }

    public int GetPlayerOwnCountnForBuildItemByType(BuildItemType buildItemType) {
        int count = 0;
        for (int index = 0; index < HotbarSize; index++) {
            GameItem currentItem = Hotbar[index];
            if (currentItem is BuildItem buildItem && buildItem.Type == buildItemType) {
                count++;
            }
        }
        return count;

    }
}