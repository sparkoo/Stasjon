using Godot;
using System;

public class LevelTemplate : Node {

  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

  public override void _Ready() {
    randomizeGround();
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
  }

  private void randomizeGround() {
    var groundBlocks = GetNode("Field").GetChildren();
    foreach (var groundBlock in groundBlocks) {
      var mesh = (MeshInstance)groundBlock;
      // copy the material to be able to set different seeds
      var materialCopy = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);

      var noise = (OpenSimplexNoise)((NoiseTexture)materialCopy.RoughnessTexture).Noise;

      noise.Seed = rnd.Next();

      mesh.SetSurfaceMaterial(0, materialCopy);
    }
  }

}
