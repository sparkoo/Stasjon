using System;
using System.Collections.Generic;
using Godot;

public partial class Tile : Spatial, ClickableItem {
  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);

  public event ItemClicked objectClicked;

  private SpatialMaterial material;
  public int index { get; private set; }
  public bool hasDepot { get { return depot != null; } }
  public DepotTemplate depot { get; private set; }

  public override void _Ready() {
    this.Connect("input_event", this, nameof(clickedOnEmptyTile));

    index = int.Parse(Name.Substring("Tile".Length));
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      registerListener(item);

      var depotMaybe = item as DepotTemplate;
      if (depotMaybe != null) {
        this.depot = depotMaybe;
      }
    }
    randomizeGround();
  }

  private void clickedOnEmptyTile(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {
        objectClicked?.Invoke(new PathBuildElement(null, index));
      }
    }
  }

  public void placeItem(Node o) {
    GetNode("Items").AddChild(o);
    registerListener(o);
  }

  private void registerListener(object o) {
    if (o is ClickableItem) {
      ((ClickableItem)o).objectClicked += clickedOnObject;
    } else {
      GD.PushWarning(String.Format("Trying to subscribe to object that is no ClickableItem. '{0}', type '{1}'", o, o.GetType()));
    }
  }

  private void clickedOnObject(PathBuildElement pathElement) {
    GD.Print("invoke Tile.clickedOnObject");
    objectClicked?.Invoke(new PathBuildElement(pathElement.color, index));
    // highlight(!material.EmissionEnabled);
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

  public bool hasSelectableItem() {
    Godot.Collections.Array items = GetNode("Items").GetChildren();
    if (hasDepot && depot.depotType == DepotType.END) {
      return false;
    }
    if (items.Count > 0) {
      return true;
    }
    return false;
  }

  public void select(bool highlightThis = true) {
    // highlight(highlightThis);
    material.EmissionEnabled = highlightThis;
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.select();
    }
  }

  public void cancelSelect() {
    material.EmissionEnabled = false;
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.cancelSelect();
    }
  }

  public PlayColor? getItemColor() {
    foreach (PlayObject item in GetNode("Items").GetChildren()) {
      return item.color;
    }

    return null;
  }
}
