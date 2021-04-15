using System.Collections.Generic;
using Godot.Collections;

public class PathBuilder {
  private Array<Tile> tiles;
  private readonly int cols;
  private readonly int rows;
  private readonly int N;

  public PathBuilder(int cols, int rows, Array<Tile> tiles) {
    this.cols = cols;
    this.rows = rows;
    this.N = cols * rows;
    this.tiles = tiles;
  }

  // list of possible candidates for tile in given index `i`
  public IList<int> nextCandidates(int i) {
    var nextCandidates = new List<int>();

    if (tiles[i].hasDepot) {
      var direction = tiles[i].depot.direction;
      switch (direction) {
        case Direction.LEFT:
          add(i, i - 1);
          break;
        case Direction.RIGHT:
          add(i, i + 1);
          break;
        case Direction.UP:
          add(i, i - cols);
          break;
        case Direction.DOWN:
          add(i, i - cols);
          break;
        default:
          throw new System.Exception(string.Format("WTH is direction here? '{0}'", direction));
      }
    } else {
      // check if neighbours are

      // one left
      int ci = i - 1;
      if (ci >= 0 && (ci + 1) % cols != 0) {
        add(i, ci);
      }

      // one right
      ci = i + 1;
      if (ci < cols * rows && (ci) % cols != 0) {
        add(i, ci);
      }

      // one down
      ci = i + cols;
      if (ci < N) {
        add(i, ci);
      }

      // one up
      ci = i - cols;
      if (ci >= 0) {
        add(i, ci);
      }
    }

    return nextCandidates;

    // include the candidate only if
    // - empty tile
    // - rails of different color
    // - end depot of selected color
    void add(int selected, int candidateIndex) {
      if (isRailsOfDifferentColorOrEmpty() || isEndDepotOfSameColor()) {
        nextCandidates.Add(candidateIndex);
      }

      bool isRailsOfDifferentColorOrEmpty() {
        return !tiles[candidateIndex].hasDepot && tiles[candidateIndex].getItemColor() != tiles[selected].getItemColor();
      }

      bool isEndDepotOfSameColor() {
        return tiles[candidateIndex].hasDepot && isEndDepot(selected, candidateIndex);
      }
    }
  }

  private bool isEndDepot(int selected, int i) {
    var color = tiles[selected].getItemColor();
    if (color.HasValue) {
      if (tiles[i].hasDepot && tiles[i].getItemColor().Value == color.Value && tiles[i].depot.depotType == DepotType.END) {
        return true;
      }
    }
    return false;
  }
}