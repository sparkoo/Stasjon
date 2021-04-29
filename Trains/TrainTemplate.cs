using System;
using Godot;

public class TrainTemplate : Spatial, PlayObject {
  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;

  private Vector3 velocity = new Vector3(0, 0, 0);
  private PathElement currentTarget;
  private PathElement finalTarget;
  private bool move = false;

  public override void _Ready() {
  }

  public override void _PhysicsProcess(float delta) {
    base._PhysicsProcess(delta);
    if (move) {
      Translate(velocity * delta);
      var trainPos = GlobalTransform.origin;
      trainPos.y = 0;
      var targetPos = ((Spatial)currentTarget.item).GlobalTransform.origin;
      targetPos.y = 0;
      var distance = trainPos.DistanceTo(targetPos);
      // GD.Print(string.Format("train: '{0}', target: '{1}', distance: '{2}'", trainPos, targetPos, distance));
      if (distance < 0.01) {
        if (currentTarget.next != null) {
          currentTarget = currentTarget.next;
        } else {
          GD.Print(string.Format("{0} train arrived to end depot", color));
          move = false;
        }
      } else {
        LookAt(targetPos, new Vector3(0, 1, 0));
      }
    }
  }

  public void chooChoo(bool emit = true) {
    ((Particles)GetNode("Smoke")).Emitting = emit;
    ((Particles)GetNode("Smoke2")).Emitting = emit;
  }

  // TODO: ok we can't do it like this, I guess trains should go on rails
  public void go(PathElement from, PathElement to) {
    GD.Print(string.Format("train '{0}' should go from '{1}' to '{2}'", color, from, to));
    velocity = new Vector3(0, 0, -1);
    currentTarget = from;
    finalTarget = to;
    move = true;
    // GlobalTransform = destination;
  }
}
