using agame.utils;
using Godot;

namespace agame.Player;

public partial class PlayerCamera : Camera3D {
    private float pitch = 0f;
    private const float Sens = 0.002f;
    public bool freeCamEnabled = false;

    private Node3D buildPreview = null;
    private const float MinBuildPreviewDistance = -4f;
    private const float MaxBuildPreviewDistance = -6f;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion m && Input.MouseMode == Input.MouseModeEnum.Captured) {
            // yaw
            RotateY(-m.Relative.X * Sens);

            // pitch
            pitch -= m.Relative.Y * Sens;
            pitch = Mathf.Clamp(pitch, Mathf.DegToRad(-89), Mathf.DegToRad(89));

            Rotation = new Vector3(pitch, Rotation.Y, 0);
        }
        if (Input.IsActionPressed("scroll_down") && buildPreview is not null) {
            var newPosition = buildPreview.Position;
            newPosition.Z += 0.1f;
            GD.Print($"new desired z position: {newPosition.Z}");
            newPosition.Z = Mathf.Clamp(newPosition.Z, MaxBuildPreviewDistance, MinBuildPreviewDistance);
            buildPreview.Position = newPosition;
        }

        if (Input.IsActionPressed("scroll_up") && buildPreview is not null) {
            var newPosition = buildPreview.Position;
            newPosition.Z -= 0.1f;
            GD.Print($"new desired z position: {newPosition.Z}");
            newPosition.Z = Mathf.Clamp(newPosition.Z, MaxBuildPreviewDistance, MinBuildPreviewDistance);
            buildPreview.Position = newPosition;
        }
    }

    public override void _Process(double delta) {
        ToggleFreeCam();
        HandleFreeCamMovement();
        UpdateBuildPreviewPosition();
    }

    private void HandleFreeCamMovement() {
        if (freeCamEnabled) {
            Vector3 freeCamMovement = Vector3.Zero;
            if (Input.IsActionPressed("move_left")) {
                freeCamMovement.X -= 1.0f;
            }
            if (Input.IsActionPressed("move_right")) {
                freeCamMovement.X += 1.0f;
            }
            if (Input.IsActionPressed("move_forward")) {
                freeCamMovement.Z -= 1.0f;
            }
            if (Input.IsActionPressed("move_backwards")) {
                freeCamMovement.Z += 1.0f;
            }

            if (Input.IsActionPressed("up_freecam")) {
                freeCamMovement.Y += 1.0f;
            }
            if (Input.IsActionPressed("down_freecam")) {
                freeCamMovement.Y -= 1.0f;
            }

            Basis basis = GlobalBasis;
            Vector3 movement = basis.X * freeCamMovement.X + basis.Z * freeCamMovement.Z;

            Transform3D newTransform = Transform;
            newTransform.Origin += movement;
            Transform = newTransform;
        }
    }

    private void ToggleFreeCam() {
        if (Input.IsActionJustPressed("toggle_freecam")) {
            freeCamEnabled = !freeCamEnabled;
            GD.Print($"freecam is now enabled {freeCamEnabled}");
            if (!freeCamEnabled) {
                Transform = Transform3D.Identity;
            }
        }
    }

    // also despawns and spawn again <- mostly for debug purposes right now
    public void SpawnBuildPreview(string pathToScene) {
        if (buildPreview is not null) {
            GD.PrintErr("despawning build preview");
            buildPreview.Free();
            buildPreview = null;
        }
        GD.Print("spawning build preview");
        var scene = GD.Load<PackedScene>(pathToScene);
        buildPreview = scene.Instantiate<Node3D>();

        Vector3 buildPreviewPosition = new(0f, -1.5f, -3f);
        buildPreview.Position = buildPreviewPosition;

        AddChild(buildPreview);
    }

    private void UpdateBuildPreviewPosition() {
        var space = GetWorld3D().DirectSpaceState;
        if (buildPreview is null) {
            return;
        }
        Vector3 existingRotation = buildPreview.Rotation;
        existingRotation.X = -Rotation.X;
        buildPreview.Rotation = existingRotation;

        Vector3? snappedToGround = Utils.SnapToGround(buildPreview.GlobalPosition, space, [Player.Instance.GetRid()]);
        if (snappedToGround is Vector3 newPosition) {
            buildPreview.GlobalPosition = newPosition;
        }
        else {
            GD.PrintErr("couldnt find valid ground to snap build preview to");
        }
    }
}