using Godot;

public partial class World : Node3D
{
    public override void _Process(double delta)
    {
        // if (Input.IsActionJustPressed("enter"))
        // {
        //     var scene = GD.Load<PackedScene>("res://assets/scenes/dirtpatch.tscn");
        //     var instance = scene.Instantiate<Node3D>();
        //     instance.Position = new Vector3(0.0f, 1.0f, 0.0f);
        //     AddChild(instance);
        //     GD.Print("Added new dirtpatch to world");
        // }
        if (Input.IsActionJustPressed("escape"))
        {
            GD.Print("escape was pressed, mosue mode: " + Input.MouseMode);
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
            {

                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
}
