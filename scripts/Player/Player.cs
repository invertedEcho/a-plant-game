using Godot;
using Godot.Collections;

public partial class Player : CharacterBody3D {
    PlayerCamera playerCamera;
    private int _speed = 15;
    private float _jumpVelocity = 10.0f;

    public static Player Instance;

    public int CoinCount { get; set; } = 0;

    const int InventorySize = 8;

    // maybe have a better data structure, for now we want fixed size of InventorySize. for now we just ensure this at developer side that it wont be bigger than this
    private Array<GameItem> Inventory = new Array<GameItem>();

    public override void _Ready() {
        Instance = this;
        playerCamera = (PlayerCamera)GetNode("PlayerCamera");
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
        Inventory.Add(new GameItem { IsPlaceHolder = true });
    }

    public void HandleMovementInput() {
        if (playerCamera.freeCamEnabled) return;
        Vector3 localVelocity = new Vector3(0.0f, 0.0f, 0.0f);

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

        Godot.Basis basis = playerCamera.GlobalBasis;
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

    public void AddCoin(int toAdd) {
        CoinCount += toAdd;
        UiManager.Instance.CoinsLabel.Text = CoinCount.ToString();
    }

    // if there is already an item at the given index
    /// Returns a boolean indicating whether adding the item was succesful
    public bool AddItemToInventory(GameItem itemToAdd, int index) {
        if (index + 1 > InventorySize) {
            GD.PrintErr($"The inventory has a size of {InventorySize} so the given index {index} is invalid");
            return false;
        }

        GameItem currentItem = Inventory[index];

        if (!currentItem.IsPlaceHolder) {
            GD.PrintErr($"The inventory already contains a GameItem at index {index}: {currentItem}");
            return false;
        }

        Inventory[0] = itemToAdd;
        return true;
    }

    /// Tries to find an empty slot in the inventory and add. If unsucessful, this method will return false, otherwise true.
    public bool AppendItemToInventory(GameItem itemToAdd) {
        for (int index = 0; index < Inventory.Count; index++) {
            GameItem currentGameItem = Inventory[index];

            if (currentGameItem.IsPlaceHolder) {
                currentGameItem = itemToAdd;
                UiManager.Instance.InventorySlot1.Texture = GD.Load<Texture2D>(itemToAdd.PathToTexture);
                return true;
            }
        }
        return false;
    }

    /// Returns a boolean indicating whether the removal of the item at the given index was succesful
    public bool RemoveItemFromInventory(int index) {
        if (index + 1 > InventorySize) {
            GD.PrintErr($"The inventory has a size of {InventorySize} so the given index {index} is invalid");
            return false;
        }

        Inventory[index] = new GameItem { IsPlaceHolder = true };
        return true;
    }
}
