using Godot;
using System;

public class LevelTemplate : Node {
  private Field field = new Field(4, 4);
  private Godot.Collections.Array groundBlocks;
  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

  public override void _Ready() {
    groundBlocks = GetNode("Field").GetChildren();
    randomizeGround();
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
  }

  private void randomizeGround() {
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
