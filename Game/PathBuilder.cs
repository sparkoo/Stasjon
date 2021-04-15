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

  public IList<int> nextCandidates(int i) {
    var nextCandidates = new List<int>();

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

    return nextCandidates;

    void add(int selected, int candidateIndex) {
      if ((!tiles[ci].hasDepot && tiles[ci].getItemColor() != tiles[selected].getItemColor()) ||
      (tiles[ci].hasDepot && isEndDepot(selected, ci))) {
        nextCandidates.Add(ci);
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