using System;
using Godot;

public partial class Tile : Spatial, ClickableItem {

  public event ItemClicked objectClicked;

  private int index;

  public override void _Ready() {
    index = determineIndex();
    listenItems();
  }

  private int determineIndex() {
    return int.Parse(Name.Substring("Tile".Length));
  }

  private void listenItems() {
    foreach (ClickableItem item in GetNode("Items").GetChildren()) {
      item.objectClicked += clickedOnObject;
    }
  }

  private void clickedOnObject(PathElement pathElement) {
    pathElement.index = index;
    objectClicked?.Invoke(pathElement);
  }
}
