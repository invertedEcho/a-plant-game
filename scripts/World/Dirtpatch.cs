using Godot;

public partial class Dirtpatch : Node3D
{
    enum DirtPatchState
    {
        Dry,
        Watered,
        YoungCactus,
        AgedCactus,
        CactusWithFlowers
    }
    [Export]
    Label3D interactLabel;

    DirtPatchState currentDirtPatchState = DirtPatchState.Dry;
    CharacterBody3D player;

    MeshInstance3D cactus;

    public override void _Ready()
    {
        player = (CharacterBody3D)GetNode("/root/MainScene/Player");
    }

    public override void _Process(double delta)
    {
        Vector3 positionOfPlayer = player.Position;
        Vector3 positionOfDirtPatch = Position;
        float distance = Position.DistanceTo(positionOfPlayer);
        if (distance < 10f)
        {
            interactLabel.Visible = true;

            if (Input.IsActionJustPressed("interact"))
            {
                switch (currentDirtPatchState)
                {
                    case DirtPatchState.Dry:
                        currentDirtPatchState = DirtPatchState.Watered;
                        interactLabel.Text = "Press F to make it YoungCactus";
                        break;
                    case DirtPatchState.Watered:
                        currentDirtPatchState = DirtPatchState.YoungCactus;
                        if (cactus == null)
                        {
                            PackedScene scene = GD.Load<PackedScene>("res://assets/scenes/cactus.tscn");
                            MeshInstance3D instance = scene.Instantiate<MeshInstance3D>();
                            instance.Position = new Vector3(0.0f, 0.5f, 0.0f);
                            AddChild(instance);
                            cactus = instance;
                        }
                        interactLabel.Text = "Press F to make it AgedCactus";
                        break;
                    case DirtPatchState.YoungCactus:
                        currentDirtPatchState = DirtPatchState.AgedCactus;
                        cactus.Mesh = (Mesh)GD.Load("res://assets/models/cactus/Cactus_2.obj");
                        interactLabel.Text = "Press F to make it Cactus Flower";
                        break;
                    case DirtPatchState.AgedCactus:
                        cactus.Mesh = (Mesh)GD.Load("res://assets/models/cactus/CactusFlowers_2.obj");
                        interactLabel.Text = "Congratulions on your first grown cactus!";
                        break;
                }
            }
        }
        else
        {
            interactLabel.Visible = false;
        }
    }

    private string GetPathForCactus()
    {
        switch (currentDirtPatchState)
        {
            case DirtPatchState.YoungCactus:
                return "res://assets/models/cactus/Cactus_3.obj";
            case DirtPatchState.AgedCactus:
                return "res://assets/models/cactus/Cactus_2.obj";
            case DirtPatchState.CactusWithFlowers:
                return "res://assets/models/cactus/CactusFlowers_2.obj";
            default: return "";
        }
    }
}
