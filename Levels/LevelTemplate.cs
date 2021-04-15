using Godot;
using System;
using System.Collections.Generic;

public class LevelTemplate : Node {
  [Export] private int cols;
  [Export] private int rows;

  private bool complete = false;

  Dictionary<PlayColor, PackedScene> railsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsStraightBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsStraightRed.tscn")}
  };

  private IDictionary<PlayColor, Path> paths = new Dictionary<PlayColor, Path>();

  private int? selected = null;

  private Godot.Collections.Array<Tile> tiles = new Godot.Collections.Array<Tile>();

  private PathBuilder pathBuilder;

  public override void _Ready() {
    pathBuilder = new PathBuilder(cols, rows, tiles);

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

    foreach (Path path in paths.Values) {
      path.pathCompleteEvent += pathCompleteChanged;
    }

    printPaths();
  }

  //TODO: refactor
  private void clickedTile(PathBuildElement pathElement) {
    if (complete) {
      GD.Print("Hey, you've finished the level. Relax!");
      return;
    }

    GD.Print(String.Format("Level -> clicked on [{0}] from selected: '{1}'", pathElement, selected));

    // do we have anything selected?
    if (selected == null) {
      if (tiles[pathElement.index.Value].hasSelectableItem()) {
        tiles[pathElement.index.Value].select(false);
        selected = pathElement.index;
        selectCandidates(selected.Value);
      }
    } else {

      var clickedIndex = clickedOnCandidate(pathElement.index.Value);

      if (clickedIndex.HasValue) {
        GD.Print("clicked on candidate");
        var originalColor = tiles[clickedIndex.Value].getItemColor();
        if (originalColor.HasValue) {
          paths[originalColor.Value].cutPathIncluding(clickedIndex.Value);
        }

        var color = tiles[selected.Value].getItemColor();
        if (color != null) {
          GD.Print("build next block at ", clickedIndex);

          // prepare new rails or go to depot
          PlayObject newBlockItem;
          if (tiles[clickedIndex.Value].hasDepot) {
            newBlockItem = tiles[clickedIndex.Value].depot;
          } else {
            newBlockItem = (RailsTemplate)railsRes[color.Value].Instance();
            tiles[clickedIndex.Value].placeItem((Node)newBlockItem);
          }

          paths[color.Value].add(selected.Value, clickedIndex.Value, newBlockItem);

          // clear selections
          tiles[selected.Value].cancelSelect();
          cancelSelectCandidates(selected.Value);

          // select next candidates
          selected = clickedIndex;
          if (!(tiles[selected.Value].hasDepot && tiles[selected.Value].depot.depotType == DepotType.END)) {
            tiles[selected.Value].select(false);
            selectCandidates(selected.Value);
          }
        } else {
          throw new Exception(string.Format("this shoud not happen. We should be on block with item on it with color. We're on '{0}'", selected));
        }
      } else if (tiles[pathElement.index.Value].hasSelectableItem()) {  // clicked on new item
        tiles[selected.Value].cancelSelect();
        cancelSelectCandidates(selected.Value);

        tiles[pathElement.index.Value].select(false);
        selected = pathElement.index;
        paths[pathElement.color.Value].cutPath(selected.Value);
        selectCandidates(selected.Value);
      }
    }
    // printPaths();
  }

  private int? clickedOnCandidate(int clickedIndex) {
    foreach (int c in pathBuilder.nextCandidates(selected.Value)) {
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
    foreach (int candidateIndex in pathBuilder.nextCandidates(i)) {
      tiles[candidateIndex].select();
    }
  }

  private void cancelSelectCandidates(int i) {
    foreach (int candidateIndex in pathBuilder.nextCandidates(i)) {
      tiles[candidateIndex].cancelSelect();
    }
  }

  private void pathCompleteChanged(bool complete, PlayColor color) {
    GD.Print("pathCompleteChanged");
    if (complete) {
      getTrain(color).chooChoo();
      if (checkAllPathsCompeted()) {
        levelComplete();
      }
    } else {
      getTrain(color).chooChoo(false);
    }
  }

  private bool checkAllPathsCompeted() {
    foreach (var path in paths.Values) {
      if (!path.complete) {
        return false;
      }
    }

    return true;
  }

  private void levelComplete() {
    GD.Print("WIN \\o/");
    complete = true;
    // TODO: run trains to end depots, block level, GUI to next level
    foreach (TrainTemplate train in GetNode("Trains").GetChildren()) {
      StaticBody endDepot = (StaticBody)paths[train.color].endDepot.item;

      // TODO: this is very unusable, trains just jump into end depots
      train.go(endDepot.GlobalTransform);
    }
  }

  private TrainTemplate getTrain(PlayColor color) {
    foreach (TrainTemplate train in GetNode("Trains").GetChildren()) {
      if (train.color == color) {
        return train;
      }
    }
    return null;
  }
}
