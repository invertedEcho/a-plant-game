using agame.utils;
using Godot;

namespace agame.player;

public partial class PlayerBuildMode : Node3D {
    private const float MinBuildPreviewDistance = -4f;
    private const float MaxBuildPreviewDistance = -6f;
    private Node3D buildPreview = null;

    private Player.Player _player;

    public override void _Ready() {
        _player = GetParent<Player.Player>();
    }

    public override void _Process(double delta) {
        HandleBuildPreviewSpawnInput();

        bool inBuildMode = buildPreview != null;

        if (inBuildMode && Input.IsActionJustPressed("interact")) {
            var buildPosition = buildPreview.GlobalPosition;
            GD.Print($"building grow plot at {buildPosition}");
            World.World.Instance.PlaceGrowPlot(buildPosition);
            buildPreview.Free();
            buildPreview = null;
            _player.Inventory.RemoveItemFromHotbar(_player.Inventory.CurrentHotbarIndex);
        }

        UpdateBuildPreviewPosition();
    }

    public override void _Input(InputEvent @event) {
        bool inBuildMode = buildPreview != null;
        if (Input.IsMouseButtonPressed(MouseButton.WheelDown) && inBuildMode) {
            var newPosition = buildPreview.Position;
            newPosition.Z += 0.1f;
            newPosition.Z = Mathf.Clamp(newPosition.Z, MaxBuildPreviewDistance, MinBuildPreviewDistance);
            buildPreview.Position = newPosition;
        }

        if (Input.IsMouseButtonPressed(MouseButton.WheelUp) && inBuildMode) {
            var newPosition = buildPreview.Position;
            newPosition.Z -= 0.1f;
            newPosition.Z = Mathf.Clamp(newPosition.Z, MaxBuildPreviewDistance, MinBuildPreviewDistance);
            buildPreview.Position = newPosition;
        }
    }

    private void HandleBuildPreviewSpawnInput() {
        if (Input.IsActionJustPressed("build_mode")) {
            if (buildPreview == null) {
                var pathToPreviewScene = _player.GetPathToPreviewScene();
                if (pathToPreviewScene is not null) {
                    ToggleBuildPreview(pathToPreviewScene);
                    UiManager.Instance.InteractLabel.Text = "Press (E) to exit Build Mode | Press (F) to build here";
                }
            }
            else {
                UiManager.Instance.InteractLabel.Text = "Press (E) to enter Build Mode";
            }
        }
    }

    // also despawns and spawn again <- mostly for debug purposes right now
    public void ToggleBuildPreview(string pathToScene) {
        if (buildPreview == null) {
            GD.Print("spawning build preview");
            var scene = GD.Load<PackedScene>(pathToScene);
            buildPreview = scene.Instantiate<Node3D>();

            Vector3 buildPreviewPosition = new(0f, -1.5f, -3f);
            buildPreview.Position = buildPreviewPosition;

            // add as child of player camera so we get auto rotation/position relative to camera
            _player.PlayerCamera.AddChild(buildPreview);
        }
        else {
            buildPreview.Free();
            buildPreview = null;
        }
    }

    public void DespawnBuildPreview() {
        if (buildPreview is not null) {
            GD.PrintErr("despawning build preview");
            buildPreview.Free();
            buildPreview = null;
        }
    }

    private void UpdateBuildPreviewPosition() {
        if (buildPreview is null) {
            return;
        }

        var space = GetWorld3D().DirectSpaceState;
        Vector3 existingRotation = buildPreview.Rotation;
        existingRotation.X = -_player.PlayerCamera.Rotation.X;
        buildPreview.Rotation = existingRotation;

        Vector3? snappedToGround = Utils.SnapToGround(buildPreview.GlobalPosition, space, [Player.Player.Instance.GetRid()]);
        if (snappedToGround is Vector3 newPosition) {
            buildPreview.GlobalPosition = newPosition;
        }
        else {
            GD.PrintErr("couldnt find valid ground to snap build preview to");
        }
    }

    public bool GetIsInBuildMode() {
        return buildPreview != null;
    }
}