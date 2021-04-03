using System;
using Godot;

public partial class Tile : Spatial {
  private int index;

  [Signal]
  public delegate void ClickedTile(int index);

  public override void _Ready() {
    index = determineIndex();
    listenItems();
  }

  private int determineIndex() {
    return int.Parse(Name.Substring("Tile".Length));
  }

  private void listenItems() {
    foreach (Node item in GetNode("Items").GetChildren()) {
      item.Connect("ClickedSignal", this, nameof(clickedOnObject));
    }
  }

  private void clickedOnObject() {
    GD.Print(String.Format("Received signal on [{0}]", index));
    EmitSignal(nameof(ClickedTile), index);
  }
}