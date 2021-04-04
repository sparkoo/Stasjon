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
    foreach (int i in nextCandidates(selected)) {
      var candidateTile = tiles[i];
      if (candidateTile.nextCandidate(PlayColor.NONE)) {
        candidates.Add(candidateTile.index);
      }
    }
  }

  private IList<int> nextCandidates(int i) {
    var nextCandidates = new List<int>();

    GD.Print(String.Format("testing '{0}', '{1}'", i - 1, cols));
    if (i - 1 >= 0 && (i) % cols != 0) {
      nextCandidates.Add(i - 1);
    }

    GD.Print(String.Format("testing '{0}', '{1}'", i + 1, cols));
    if (i + 1 < cols * rows && (i + 1) % cols != 0) {
      nextCandidates.Add(i + 1);
    }

    if (i + cols < N()) {
      nextCandidates.Add(i + cols);
    }

    if (i - cols >= 0) {
      nextCandidates.Add(i - cols);
    }

    return nextCandidates;
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
  }

  private int N() {
    return cols * rows;
  }
}
