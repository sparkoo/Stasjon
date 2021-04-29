using Godot;

public class DepotTemplate : StaticBody, ClickableItem, PlayObject {
  public event ItemClicked objectClicked;

  [Export] private string name = "";
  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;
  [Export] public DepotType depotType { get; private set; } = DepotType.START;

  //TODO: don't set manually but calculate from rotation
  [Export] public Direction direction { get; private set; }

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
        objectClicked?.Invoke(new PathBuildElement(color, null));
      }
    } else if (@event is InputEventMouseMotion) {
      var motion = (InputEventMouseMotion)@event;
      if (motion.ButtonMask == (int)Godot.ButtonList.MaskLeft) {
        objectClicked?.Invoke(new PathBuildElement(color, null));
      }
    }
  }

  public void select(bool highlightThis = true) {
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
