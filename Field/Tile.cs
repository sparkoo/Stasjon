using System;
using System.Collections.Generic;
using Godot;

public partial class Tile : Spatial, ClickableItem {
  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

  public event ItemClicked objectClicked;

  private SpatialMaterial material;
  public int index { get; private set; }
  private bool hasDepot;
  private bool candidate;

  public override void _Ready() {
    index = int.Parse(Name.Substring("Tile".Length));
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.objectClicked += clickedOnObject;
      if (item is DepotTemplate) {
        hasDepot = true;
      }
    }
    randomizeGround();
  }

  private void clickedOnObject(PathElement pathElement) {
    pathElement.index = index;
    objectClicked?.Invoke(pathElement);
    highlight(!material.EmissionEnabled);
  }

  private void highlight(bool highlight) {
    material.EmissionEnabled = highlight;
  }

  public void cancelCandidate() {
    candidate = false;
    highlight(false);
  }

  public Boolean nextCandidate() {
    if (!hasDepot) {
      candidate = true;
      highlight(true);
      return true;
    } else {
      return false;
    }
  }

  private void randomizeGround() {
    var mesh = (MeshInstance)this.GetNode("TileMesh");
    // copy the material to be able to set different seeds
    material = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);
    var noise = (OpenSimplexNoise)((NoiseTexture)material.RoughnessTexture).Noise;
    noise.Seed = rnd.Next();

    material = MaterialUtils.setClickableEmission(material);

    mesh.SetSurfaceMaterial(0, material);

  }

  public void select() {
    highlight(true);
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.select();
    }
  }

  public void cancelSelect() {
    highlight(false);
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.cancelSelect();
    }
  }
}
