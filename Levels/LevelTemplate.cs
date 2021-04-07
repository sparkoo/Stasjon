using Godot;
using System;
using System.Collections.Generic;

public class LevelTemplate : Node {
  [Export] private int cols;
  [Export] private int rows;

  Dictionary<PlayColor, PackedScene> railsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsStraightBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsStraightRed.tscn")}
  };

  private IDictionary<PlayColor, Path> paths = new Dictionary<PlayColor, Path>();

  private int? selected = null;

  private Godot.Collections.Array<Tile> tiles = new Godot.Collections.Array<Tile>();

  public override void _Ready() {
    foreach (Tile tile in GetNode("Field").GetChildren()) {
      tiles.Add(tile);
    }
    connectTileEvents();
    initPaths();
  }

  private void connectTileEvents() {
    foreach (ClickableItem tile in tiles) {
      tile.objectClicked += clickedTile;
    }
  }

  private void initPaths() {
    // go through all tiles
    foreach (Tile tile in tiles) {
      // for those who has depot
      if (tile.hasDepot) {
        DepotTemplate depot = tile.depot;
        var color = depot.color;
        PathElement newPathElement = new PathElement(tile.index, depot);

        // create new Path or get existing one, based on color
        Path path;
        var hasPath = paths.TryGetValue(color, out path);
        if (!hasPath) {
          path = new Path(color);
          paths.Add(color, path);
        }

        // write depot to the path
        if (depot.depotType == DepotType.START) {
          path.startDepot = newPathElement;
        } else if (depot.depotType == DepotType.END) {
          path.endDepot = newPathElement;
        } else {
          throw new Exception(String.Format("I don't know this depot type '{0}'.", depot.depotType));
        }
      }
    }
    printPaths();
  }

  private void clickedTile(PathBuildElement pathElement) {
    GD.Print(String.Format("Level -> clicked on [{0}] selected: '{1}'", pathElement, selected));

    // do we have anything selected?
    if (selected == null) {
      if (pathElement.color.HasValue) {
        tiles[pathElement.index.Value].select();
        selected = pathElement.index;
        selectCandidates(selected.Value);
      }
    } else {

      var clickedIndex = clickedOnCandidate(pathElement.index.Value);

      if (clickedIndex.HasValue) {
        GD.Print("clicked on candidate");
        var originalColor = tiles[clickedIndex.Value].getItemColor();
        if (originalColor.HasValue) {
          paths[originalColor.Value].cleanPathIncluding(clickedIndex.Value);
        }

        // get the color of selected
        var color = tiles[selected.Value].getItemColor();
        if (color != null) {
          GD.Print("build next block at ", clickedIndex);
          var newRails = (RailsTemplate)railsRes[color.Value].Instance();
          tiles[clickedIndex.Value].placeItem(newRails);
          paths[color.Value].add(selected.Value, clickedIndex.Value, newRails);


          tiles[selected.Value].cancelSelect();
          cancelSelectCandidates(selected.Value);

          selected = clickedIndex;
          tiles[selected.Value].select(false);
          selectCandidates(selected.Value);
        } else {
          throw new Exception(string.Format("this shoud not happen. We should be on block with item on it with color. We're on '{0}'", selected));
        }
      } else if (pathElement.color.HasValue) {  // clicked on new item
        tiles[selected.Value].cancelSelect();
        cancelSelectCandidates(selected.Value);

        tiles[pathElement.index.Value].select();
        selected = pathElement.index;
        paths[pathElement.color.Value].cleanPath(selected.Value);
        selectCandidates(selected.Value);
      }
    }
    printPaths();
  }

  private int? clickedOnCandidate(int clickedIndex) {
    foreach (int c in nextCandidates(selected.Value)) {
      if (c == clickedIndex) {
        return c;
      }
    }

    return null;
  }

  private void printPaths() {
    GD.Print("paths");
    foreach (Path p in paths.Values) {
      GD.Print(p.ToString());
    }
  }

  private void selectCandidates(int i) {
    foreach (int candidateIndex in nextCandidates(i)) {
      tiles[candidateIndex].select();
    }
  }

  private void cancelSelectCandidates(int i) {
    foreach (int candidateIndex in nextCandidates(i)) {
      tiles[candidateIndex].cancelSelect();
    }
  }

  private IList<int> nextCandidates(int i) {
    var nextCandidates = new List<int>();

    if (i - 1 >= 0 && (i) % cols != 0 && !tiles[i - 1].hasDepot) {
      nextCandidates.Add(i - 1);
    }

    if (i + 1 < cols * rows && (i + 1) % cols != 0 && !tiles[i + 1].hasDepot) {
      nextCandidates.Add(i + 1);
    }

    if (i + cols < N() && !tiles[i + cols].hasDepot) {
      nextCandidates.Add(i + cols);
    }

    if (i - cols >= 0 && !tiles[i - cols].hasDepot) {
      nextCandidates.Add(i - cols);
    }

    return nextCandidates;
  }

  private int N() {
    return cols * rows;
  }
}
