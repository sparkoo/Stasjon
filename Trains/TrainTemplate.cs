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
}
