using Godot;

public class DepotTemplate : StaticBody, ClickableItem, PlayObject {
  public event ItemClicked objectClicked;

  [Export] private string name = "";
  [Export] public PlayColor color { get; set; } = PlayColor.NONE;
  [Export] public DepotType depotType { get; private set; } = DepotType.START;

  private bool selected = false;
  private SpatialMaterial material;

  public override void _Ready() {
    this.Connect("input_event", this, nameof(clicked));
    MaterialUtils.uniqueMaterialWithClickableEmission((MeshInstance)GetNode("DepotMesh"), out material);
  }

  private void clicked(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {
        if (depotType == DepotType.START) {
          GD.Print("invoke DepotTemplate.clicked()");
          objectClicked?.Invoke(new PathBuildElement(color, null));
        }
      }
    }
  }

  public void select() {
    selected = true;
    material.EmissionEnabled = selected;
  }

  public void cancelSelect() {
    selected = false;
    material.EmissionEnabled = selected;
  }

  public PlayColor getColor() {
    return color;
  }
}
