using System;
using System.Collections.Generic;
using Godot;

public partial class Tile : Spatial, ClickableItem {
  private static readonly Random rnd = new Random(DateTime.Now.Millisecond);
  PackedScene rails = (PackedScene)GD.Load("res://Rails/RailsStraightBlue.tscn");

  public event ItemClicked objectClicked;

  private SpatialMaterial material;
  public int index { get; private set; }
  private bool hasDepot;

  private bool candidate;
  private PlayColor candidateColor = PlayColor.NONE;

  public override void _Ready() {
    this.Connect("input_event", this, nameof(clicked));

    index = int.Parse(Name.Substring("Tile".Length));
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.objectClicked += clickedOnObject;
      registerListener(item);
      if (item is DepotTemplate) {
        hasDepot = true;
      }
    }
    randomizeGround();
  }

  private void clicked(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
    if (@event is InputEventMouseButton) {
      var click = (InputEventMouseButton)@event;
      if (click.Pressed) {
        if (candidate) {
          GD.Print("build next block at ", index);
          var newRails = rails.Instance();
          this.GetNode("Items").AddChild(newRails);
          registerListener(newRails);
          var pathElement = new PathElement(candidateColor, index);
          clickedOnObject(pathElement);
        }
      }
    }
  }

  private void registerListener(object o) {
    if (o is ClickableItem) {
      ((ClickableItem)o).objectClicked += clickedOnObject;
    } else {
      GD.PushWarning(String.Format("Trying to subscribe to object that is no ClickableItem. '{0}', type '{1}'", o, o.GetType()));
    }
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
    candidateColor = PlayColor.NONE;
    highlight(false);
  }

  public Boolean nextCandidate(PlayColor color) {
    if (!hasDepot) {
      candidate = true;
      candidateColor = color;
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
