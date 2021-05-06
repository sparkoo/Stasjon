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
      // we do not care if we're out of field. So hopefuly we do not put the depot in unreachable direction which would cause crash.
      // TODO: check?
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
          add(i, i + cols);
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
        return tiles[candidateIndex].hasDepot && isEndDepot();

        bool isEndDepot() {
          var color = tiles[selected].getItemColor();
          if (color.HasValue) {
            if (tiles[candidateIndex].hasDepot &&
            tiles[candidateIndex].getItemColor().Value == color.Value &&
            tiles[candidateIndex].depot.depotType == DepotType.END) {
              // the candidate is end depot of correct color, but are we from correct direction?
              var depotDirection = tiles[candidateIndex].depot.direction;
              switch (depotDirection) {
                case Direction.LEFT:
                  return selected + 1 == candidateIndex;
                case Direction.RIGHT:
                  return selected - 1 == candidateIndex;
                case Direction.UP:
                  return selected + cols == candidateIndex;
                case Direction.DOWN:
                  return selected - cols == candidateIndex;
                default:
                  throw new System.Exception(string.Format("WTH is direction here? '{0}'", depotDirection));
              }
            }
          }
          return false;
        }
      }
    }
  }
}
