using System;
using Godot;

public partial class RailsTemplate : StaticBody, ClickableItem {
  public event ItemClicked objectClicked;

  public override void _Ready() {
    this.Connect("input_event", this, nameof(clicked));
    // uniqueMaterial();
  }

  private void clicked(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {
        objectClicked?.Invoke(new PathBuildElement(PlayColor.BLUE, null));
      }
    }
  }

  public void cancelSelect() {
    GD.Print("unselect rail");
  }

  public void select() {
    GD.Print("select rail");
  }
}