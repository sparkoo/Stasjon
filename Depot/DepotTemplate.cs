using Godot;
using System;

public class DepotTemplate : StaticBody {
  private bool selected = false;
  private SpatialMaterial material;

  [Export]
  private string name = "";
  public override void _Ready() {
    this.Connect("input_event", this, nameof(clicked));
    uniqueMaterial();
  }
  private void clicked(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    // Replace with function body.
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {
        GD.Print("Clicked on ", this.name + " pico");
        select(!selected);
      }
    }
  }

  private void select(bool select) {
    selected = select;
    material.EmissionEnabled = selected;
  }

  private void uniqueMaterial() {
    var mesh = (MeshInstance)GetNode("DepotMesh");
    // copy the material to be able to set different seeds
    material = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);
    MaterialUtils.setClickableEmission(material);

    mesh.SetSurfaceMaterial(0, material);
  }
}
