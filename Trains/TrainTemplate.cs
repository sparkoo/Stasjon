using System;
using Godot;

public class TrainTemplate : Spatial, PlayObject {
  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;

  private Vector3 velocity = new Vector3(0, 0, 0);

  public override void _Ready() {
  }

  public override void _PhysicsProcess(float delta) {
    base._PhysicsProcess(delta);
    Translate(velocity * delta);
  }

  public void chooChoo(bool emit = true) {
    ((Particles)GetNode("Smoke")).Emitting = emit;
    ((Particles)GetNode("Smoke2")).Emitting = emit;
  }

  // TODO: ok we can't do it like this, I guess trains should go on rails
  public void go(PathElement from, PathElement to) {
    GD.Print(string.Format("train '{0}' should go from '{1}' to '{2}'", color, from, to));
    velocity = new Vector3(0, 0, 1);
    // GlobalTransform = destination;
  }
}
