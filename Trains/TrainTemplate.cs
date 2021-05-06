using System.Collections.Generic;
using Godot;

public class TrainTemplate : Spatial, PlayObject {
  [Export] public PlayColor color { get; private set; } = PlayColor.NONE;

  private readonly float SPEED = 0.5F;
  private readonly float DISTANCE_DELTA = 0.01F;

  private Vector3 velocity;
  private Queue<Vector3> trainRoute;
  private bool move = false;

  public override void _Ready() {
    velocity = new Vector3(0, 0, -SPEED);
  }

  public override void _PhysicsProcess(float delta) {
    base._PhysicsProcess(delta);
    if (move) {
      Translate(velocity * delta);

      var trainPos = GlobalTransform.origin;
      trainPos.y = 0;

      var targetPos = trainRoute.Peek();
      targetPos.y = 0;

      var distance = trainPos.DistanceTo(targetPos);
      if (distance < DISTANCE_DELTA) {
        trainRoute.Dequeue();
        if (trainRoute.Count == 0) {
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

  public void go(PathElement from, PathElement to) {
    GD.Print(string.Format("train '{0}' should go from '{1}' to '{2}'", color, from, to));
    prepareTrainRoute(from, to);
    move = true;
    // GlobalTransform = destination;
  }

  private void prepareTrainRoute(PathElement from, PathElement to) {
    trainRoute = new Queue<Vector3>();
    var currentPathElement = from;
    while (currentPathElement != null) {
      if (currentPathElement.item is RailsTemplate) {
        var rails = (RailsTemplate)currentPathElement.item;
        foreach (Position3D point in rails.GetNode("NavPoints").GetChildren()) {
          trainRoute.Enqueue(point.GlobalTransform.origin);
        }
      }
      if (currentPathElement.item is DepotTemplate) {
        var depot = (DepotTemplate)currentPathElement.item;
        trainRoute.Enqueue(depot.GlobalTransform.origin);
      }
      currentPathElement = currentPathElement.next;
    }
  }
}
