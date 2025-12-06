using Godot;
using agame.Items;
using static agame.Items.BuildItem;

namespace agame.Player;

public partial class Player : CharacterBody3D {

    // --- Player camera ---
    private PlayerCamera _playerCamera;
    private float _cameraJiggleAmmount = 0.3f;
    private Vector3 _cameraInitalPos;
    private Vector3 _cameraTargetPos;
    private bool _wasOnFloor = false;

    // --- Grapple gun --- (TODO: move somewhere else)
    private Vector3? _grappleHit = null;
    private float _grappleStren = 80.0f;
    private Rope _rope = null;

    // --- Movement related ---
    private PlayerMovement _playerMovement;

    public PlayerInventory Inventory;
    public bool InBuildMode = false;

    public static Player Instance { get; private set; }

    public float CoinCount { get; private set; } = 0f;

    public override void _Ready() {
        Instance = this;
        _playerMovement = new PlayerMovement();
        Inventory = new PlayerInventory();
        AddChild(Inventory);
        _playerCamera = (PlayerCamera)GetNode("PlayerCamera");
        _cameraTargetPos = _playerCamera.Position;
        _cameraInitalPos = _playerCamera.Position;
    }

    public override void _PhysicsProcess(double delta) {
        _playerMovement.HandleMovement(IsOnFloor(), (float)delta);
        Velocity = _playerMovement.finalVelocity;
        MoveAndSlide();

        // Check for change
        if (IsOnFloor() != _wasOnFloor) {
            if (_wasOnFloor)
                OnJumpStart();
            else
                OnJumpImpact();
        }
        _wasOnFloor = IsOnFloor();
    }

    public override void _Process(double delta) {
        HandleBuildPreviewSpawnInput();
        if (Input.IsActionJustPressed("hotbar_1")) {
            UpdateCurrentHotbarSlotSelected(0);
        }
        else if (Input.IsActionJustPressed("hotbar_2")) {
            UpdateCurrentHotbarSlotSelected(1);
        }
        else if (Input.IsActionJustPressed("hotbar_3")) {
            UpdateCurrentHotbarSlotSelected(2);
        }
        else if (Input.IsActionJustPressed("hotbar_4")) {
            UpdateCurrentHotbarSlotSelected(3);
        }
        else if (Input.IsActionJustPressed("hotbar_5")) {
            UpdateCurrentHotbarSlotSelected(4);
        }
        else if (Input.IsActionJustPressed("hotbar_6")) {
            UpdateCurrentHotbarSlotSelected(5);
        }
        else if (Input.IsActionJustPressed("hotbar_7")) {
            UpdateCurrentHotbarSlotSelected(6);
        }
        else if (Input.IsActionJustPressed("hotbar_8")) {
            UpdateCurrentHotbarSlotSelected(7);
        }

        // --- Read input from player ---
        _playerMovement.HandleInput(_playerCamera.Basis);

        // --- Camera control ---
        _playerCamera.Position = _playerCamera.Position.Lerp(_cameraTargetPos, (float)delta * 10f);

        // --- Grapple gun ---
        if (!InBuildMode) {
            if (Input.IsActionJustPressed("mouse_click_left")) {
                _grappleHit = GetCamRaycast(_playerCamera, 3000.0f);
                if (_grappleHit != null) {
                    PackedScene ropeScene = GD.Load<PackedScene>("res://assets/scenes/rope.tscn");
                    _rope = ropeScene.Instantiate<Rope>();
                    GetTree().CurrentScene.AddChild(_rope);
                }
            }
            if (Input.IsActionJustReleased("mouse_click_left")) {
                _grappleHit = null;
                if (_rope != null)
                    _rope.QueueFree();
            }

            if (_grappleHit.HasValue) {
                _rope.startPoint = _playerCamera.GlobalPosition - new Vector3(0f, 0.3f, 0f);
                _rope.endPoint = _grappleHit.Value;

                Vector3 toGrapple = _grappleHit.Value - _playerCamera.GlobalPosition;
                toGrapple = toGrapple.Normalized();
                _playerMovement.finalVelocity += toGrapple * (float)delta * _grappleStren * _playerCamera.Position.DistanceTo(_grappleHit.Value) / 10.0f;
            }
        }
    }

    private void UpdateCurrentHotbarSlotSelected(int newHotbarSlotSelected) {
        Inventory.CurrentHotbarSlotSelected = newHotbarSlotSelected;
        UiManager.Instance.UpdateSelectedHotbarSlot(newHotbarSlotSelected);
        UpdateInteractLabel();
    }

    private void UpdateInteractLabel() {
        if (InBuildMode) return;
        bool currentHotbarItemIsGrowPlot = Inventory.Hotbar[Inventory.CurrentHotbarSlotSelected] is BuildItem buildItem && buildItem.Type == BuildItemType.GrowPlot;
        if (currentHotbarItemIsGrowPlot) {
            UiManager.Instance.InteractLabel.Visible = true;
            UiManager.Instance.InteractLabel.Text = "Press (E) to enter Build Mode";
        }
        else {
            UiManager.Instance.InteractLabel.Visible = false;
            UiManager.Instance.InteractLabel.Text = "";
        }
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


    // --- Jumping callbacks ---
    private void OnJumpImpact() {
        DoCameraImpact();
    }

    private void OnJumpStart() {

    }

    private async void DoCameraImpact() {
        _cameraTargetPos = _cameraInitalPos - new Vector3(0.0f, 0.5f, 0.0f);
        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        _cameraTargetPos = _cameraInitalPos;
    }

    private Vector3? GetCamRaycast(Camera3D camera, float maxDistance) {
        PhysicsRayQueryParameters3D parameters = new PhysicsRayQueryParameters3D {
            From = camera.GlobalPosition,
            To = camera.GlobalPosition + -camera.GlobalTransform.Basis.Z * maxDistance,
            CollideWithAreas = true,
            CollideWithBodies = true
        };
        parameters.Exclude = new Godot.Collections.Array<Rid> { camera.GetCameraRid() };
        var spaceState = GetWorld3D().DirectSpaceState;
        Godot.Collections.Dictionary result = spaceState.IntersectRay(parameters);
        if (result.Count > 0)
            return (Vector3)result["position"];
        return null;
    }

    private void HandleBuildPreviewSpawnInput() {
        if (Input.IsActionJustPressed("build_mode") && !InBuildMode) {
            var pathToPreviewScene = GetPathToPreviewScene();
            if (pathToPreviewScene is not null) {
                _playerCamera.SpawnBuildPreview(pathToPreviewScene);
                InBuildMode = true;
                UiManager.Instance.InteractLabel.Text = "Pres (F) to build here";
            }
        }
    }

#nullable enable
    private string? GetPathToPreviewScene() {
        bool currentHotbarItemIsGrowPlot = Inventory.Hotbar[Inventory.CurrentHotbarSlotSelected] is BuildItem buildItem && buildItem.Type == BuildItemType.GrowPlot;
        if (currentHotbarItemIsGrowPlot) {
            return "res://assets/scenes/grow_plot_blueprint.tscn";
        }
        return null;
    }

    // if valid positon, set interact label to press fo to build thing here
}
