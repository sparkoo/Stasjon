using Godot;
using System;

public class LevelTemplate : Node {
  private Godot.Collections.Array tiles;
  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

  public override void _Ready() {
    tiles = GetNode("Field").GetChildren();
    connectTileEvents();
    randomizeGround();
  }

  private void connectTileEvents() {
    foreach (ClickableItem tile in tiles) {
      tile.objectClicked += clickedTile;
    }
  }

  private void clickedTile(PathElement pathElement) {
    GD.Print(String.Format("Level -> clicked on [{0}]", pathElement));
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
  }

  private void randomizeGround() {
    foreach (Node tile in tiles) {
      var mesh = (MeshInstance)tile.GetNode("TileMesh");
      // copy the material to be able to set different seeds
      var materialCopy = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);

      var noise = (OpenSimplexNoise)((NoiseTexture)materialCopy.RoughnessTexture).Noise;

      noise.Seed = rnd.Next();

      mesh.SetSurfaceMaterial(0, materialCopy);
    }
  }

}
