using Godot;

public partial class Player : CharacterBody3D
{
    Camera3D playerCamera;
    private int _speed = 15;
    private float _jumpVelocity = 10.0f;

    public override void _Ready()
    {
        playerCamera = (Camera3D)GetNode("PlayerCamera");
    }
    public void HandleMovementInput()
    {
        Vector3 localVelocity = new Vector3(0.0f, 0.0f, 0.0f);

        if (Input.IsActionPressed("move_left"))
        {
            localVelocity.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_right"))
        {
            localVelocity.X += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            localVelocity.Z -= 1.0f;
        }
        if (Input.IsActionPressed("move_backwards"))
        {
            localVelocity.Z += 1.0f;
        }

        localVelocity = localVelocity.Normalized();

        Godot.Basis basis = playerCamera.GlobalBasis;
        Vector3 movement = basis.X * localVelocity.X + basis.Z * localVelocity.Z;

        Velocity = new Vector3(movement.X * _speed, Velocity.Y, movement.Z * _speed);

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            Velocity = new Vector3(Velocity.X, _jumpVelocity, Velocity.Z);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyGravity(delta);
        HandleMovementInput();
        MoveAndSlide();
    }

    private void ApplyGravity(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity += new Vector3(0.0f, -30.0f * (float)delta, 0.0f);
        }
        else if (Velocity.Y < 0)
        {
            Velocity = new Vector3(Velocity.X, 0f, Velocity.Z);
        }
    }
}
