using Godot;

public partial class PlayerCamera : Camera3D {
    private float pitch = 0f;
    private const float Sens = 0.002f;
    public bool freeCamEnabled = false;

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
    }

    public override void _Process(double delta) {
        ToggleFreeCam();
        HandleFreeCamMovement();
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

            Godot.Basis basis = GlobalBasis;
            Vector3 movement = basis.X * freeCamMovement.X + basis.Z * freeCamMovement.Z;

            Transform3D newTransform = Transform;
            newTransform.Origin += freeCamMovement;
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
}
