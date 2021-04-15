using System;
using Godot;

public class TrainTemplate : KinematicBody, PlayObject {
  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;

  public override void _Ready() {
  }

  public void chooChoo(bool emit = true) {
    ((Particles)GetNode("Smoke")).Emitting = emit;
    ((Particles)GetNode("Smoke2")).Emitting = emit;
  }

  // TODO: ok we can't do it like this, I guess trains should go on rails
  public void go(Transform destination) {
    GD.Print(string.Format("train '{0}' has global transform '{1}' and move to '{2}'", color, GlobalTransform, destination));
    GlobalTransform = destination;
  }
}
