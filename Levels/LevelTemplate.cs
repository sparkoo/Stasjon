using Godot;
using System;
using System.Collections.Generic;

public class LevelTemplate : Node {
  [Export] private int cols;
  [Export] private int rows;

  private int selected = 0;
  private IList<int> candidates = new List<int>();

  private Godot.Collections.Array<Tile> tiles = new Godot.Collections.Array<Tile>();

  public override void _Ready() {
    foreach (Tile tile in GetNode("Field").GetChildren()) {
      tiles.Add(tile);
    }
    connectTileEvents();
  }

  private void connectTileEvents() {
    foreach (ClickableItem tile in tiles) {
      tile.objectClicked += clickedTile;
    }
  }

  private void clickedTile(PathElement pathElement) {
    GD.Print(String.Format("Level -> clicked on [{0}]", pathElement));

    tiles[selected]?.cancelSelect();
    foreach (int candidateIndex in candidates) {
      tiles[candidateIndex].cancelCandidate();
    }
    candidates.Clear();

    selected = pathElement.index;
    tiles[selected].select();
    var candidateTile = tiles[selected + 1];
    if (candidateTile.nextCandidate()) {
      candidates.Add(candidateTile.index);
    }
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
  }
}
