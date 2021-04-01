using Godot;
using System;

public class DepotTemplate : StaticBody {
  [Export]
  private string name = "";
  public override void _Ready() {
    this.Connect("input_event", this, nameof(_on_DepotTemplate_input_event));
  }
  private void _on_DepotTemplate_input_event(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    // Replace with function body.
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {

        GD.Print("Clicked on ", this.name + " pico");
      }
    }
  }

}
