using Godot;
using Godot.Collections;
using agame.Items;
using static agame.World.GrowPlot;
using static agame.Items.BuildItem;

namespace agame.Player;

public partial class Player : CharacterBody3D {
    private PlayerCamera _playerCamera;
    private int _speed = 15;
    private float _jumpVelocity = 10.0f;

    public static Player Instance;

    public float CoinCount { get; private set; } = 0f;


    public const int InventorySize = 8;
    // maybe have a better data structure, we want fixed size of InventorySize. for now we just ensure this at developer side that it wont be bigger than this
    private Array<GameItem> _inventory = [];

    public override void _Ready() {
        Instance = this;
        _playerCamera = (PlayerCamera)GetNode("PlayerCamera");
        for (int i = 0; i < InventorySize; i++) {
            _inventory.Add(new GameItem { IsPlaceHolder = true });
        }
    }

    private void HandleMovementInput() {
        if (_playerCamera.freeCamEnabled) return;
        Vector3 localVelocity = new();

        if (Input.IsActionPressed("move_left")) {
            localVelocity.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_right")) {
            localVelocity.X += 1.0f;
        }
        if (Input.IsActionPressed("move_forward")) {
            localVelocity.Z -= 1.0f;
        }
        if (Input.IsActionPressed("move_backwards")) {
            localVelocity.Z += 1.0f;
        }

        localVelocity = localVelocity.Normalized();

        Basis basis = _playerCamera.GlobalBasis;
        Vector3 movement = basis.X * localVelocity.X + basis.Z * localVelocity.Z;

        Velocity = new Vector3(movement.X * _speed, Velocity.Y, movement.Z * _speed);

        if (IsOnFloor() && Input.IsActionJustPressed("jump")) {
            Velocity = new Vector3(Velocity.X, _jumpVelocity, Velocity.Z);
        }
    }

    public override void _PhysicsProcess(double delta) {
        ApplyGravity(delta);
        HandleMovementInput();
        MoveAndSlide();
    }

    private void ApplyGravity(double delta) {
        if (!IsOnFloor()) {
            Velocity += new Vector3(0.0f, -30.0f * (float)delta, 0.0f);
        }
        else if (Velocity.Y < 0) {
            Velocity = new Vector3(Velocity.X, 0f, Velocity.Z);
        }
    }

    /// To substract coins, use a negative number
    public void UpdateCoin(float toAdd) {
        CoinCount += toAdd;
        UiManager.Instance.CoinsLabel.Text = CoinCount.ToString();
    }

    // if there is already an item at the given index
    /// Returns a boolean indicating whether adding the item was successful
    private bool AddItemToInventory(GameItem itemToAdd, int index) {
        if (index + 1 > InventorySize) {
            GD.PrintErr($"The inventory has a size of {InventorySize} so the given index {index} is invalid");
            return false;
        }

        GameItem currentItem = _inventory[index];

        if (!currentItem.IsPlaceHolder) {
            GD.PrintErr($"The inventory already contains a GameItem at index {index}: {currentItem}");
            return false;
        }

        _inventory[0] = itemToAdd;
        return true;
    }

    /// Tries to find an empty slot in the inventory and add the given item. If unsuccessful, this method will return false, otherwise true.
    public bool AppendItemToInventory(GameItem itemToAdd) {
        for (int index = 0; index < _inventory.Count; index++) {
            GameItem currentGameItem = _inventory[index];

            if (!currentGameItem.IsPlaceHolder) continue;

            _inventory[index] = itemToAdd;
            UiManager.Instance.InventorySlotTextures[index].Texture = GD.Load<Texture2D>(itemToAdd.PathToTexture);
            return true;
        }
        return false;
    }

    /// Returns a boolean indicating whether the removal of the item at the given index was succesful
    public bool RemoveItemFromInventory(int index) {
        if (index + 1 > InventorySize) {
            GD.PrintErr($"The inventory has a size of {InventorySize} so the given index {index} is invalid");
            return false;
        }

        _inventory[index] = new GameItem { IsPlaceHolder = true };
        GD.Print($"setting texture of inventory slot {index} to null");
        UiManager.Instance.InventorySlotTextures[index].Texture = null;
        return true;
    }

    /// Returns the game item and index for the given GameItemType
    public (PlantItem, int)? GetPlantItemByType(PlantType plantType) {
        for (int index = 0; index < InventorySize; index++) {
            GameItem currentItem = _inventory[index];
            if (currentItem is PlantItem plantItem && plantItem.Type == plantType) {
                return (plantItem, index);
            }
        }
        return null;
    }

    public int GetPlayerOwnCountForPlantItemByType(PlantType plantType) {
        int count = 0;
        for (int index = 0; index < InventorySize; index++) {
            GameItem currentItem = _inventory[index];
            if (currentItem is PlantItem plantItem && plantItem.Type == plantType) {
                count++;
            }
        }
        return count;
    }

    public int GetPlayerOwnCountnForBuildItemByType(BuildItemType buildItemType) {
        int count = 0;
        for (int index = 0; index < InventorySize; index++) {
            GameItem currentItem = _inventory[index];
            if (currentItem is BuildItem buildItem && buildItem.Type == buildItemType) {
                count++;
            }
        }
        return count;

    }
}
