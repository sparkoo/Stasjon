using System;
using Godot;

public partial class RailsTemplate : StaticBody, ClickableItem, PlayObject {
  public event ItemClicked objectClicked;

  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;

  private SpatialMaterial material;

  public override void _Ready() {
    this.Connect("input_event", this, nameof(clicked));
    MaterialUtils.uniqueMaterialWithClickableEmission((MeshInstance)GetNode("RailsMesh"), out material);
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

  public void cancelSelect() {
    material.EmissionEnabled = false;
  }

  public void select(bool highlightThis = true) {
    material.EmissionEnabled = highlightThis;
  }

  private void uniqueMaterial() {
    var mesh = (MeshInstance)GetNode("DepotMesh");
    // copy the material to be able to set different seeds
    material = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);
    MaterialUtils.setClickableEmission(material);

    mesh.SetSurfaceMaterial(0, material);
  }

  public PlayColor getColor() {
    return color;
  }
}