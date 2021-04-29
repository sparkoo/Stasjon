using Godot;
using System;
using System.Collections.Generic;

public class LevelTemplate : Node {
#pragma warning disable 0649
  // set in scene
  [Export] private int cols;
  [Export] private int rows;
#pragma warning restore 0649

  private bool complete = false;

  Dictionary<PlayColor, PackedScene> constructionRailsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsConstructionBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsConstructionRed.tscn")}
  };

  Dictionary<PlayColor, PackedScene> straightRailsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsStraightBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsStraightRed.tscn")}
  };

  Dictionary<PlayColor, PackedScene> leftRailsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsLeftBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsLeftRed.tscn")}
  };

  Dictionary<PlayColor, PackedScene> rightRailsRes = new Dictionary<PlayColor, PackedScene>() {
    {PlayColor.BLUE, (PackedScene)GD.Load("res://Rails/RailsRightBlue.tscn")},
    {PlayColor.RED, (PackedScene)GD.Load("res://Rails/RailsRightRed.tscn")}
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
      return;
    }

    // just holding over tile or clicked on same tile => do nothing
    if (selected != null && pathElement.index == selected.Value) {
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
          makeLastBlockConstruction(originalColor.Value);
        }

        var color = tiles[selected.Value].getItemColor();
        if (color != null) {
          GD.Print("build next block at ", clickedIndex);

          // prepare new rails or go to depot
          PlayObject newBlockItem;
          if (tiles[clickedIndex.Value].hasDepot) {
            newBlockItem = tiles[clickedIndex.Value].depot;
            updateLastRail(selected.Value, clickedIndex.Value, color.Value);
          } else {
            newBlockItem = buildNewConstructionRails(clickedIndex.Value, color.Value);
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
        makeLastBlockConstruction(pathElement.color.Value);
        selectCandidates(selected.Value);
      }
    }
    // printPaths();
  }

  private PlayObject buildNewConstructionRails(int tileIndex, PlayColor color) {
    var newBlockItem = (RailsTemplate)constructionRailsRes[color].Instance();

    rotateNewRails((Spatial)newBlockItem, selected.Value, tileIndex);
    updateLastRail(selected.Value, tileIndex, color);
    tiles[tileIndex].placeItem((Spatial)newBlockItem);

    return newBlockItem;
  }

  private void makeLastBlockConstruction(PlayColor color) {
    var lastElementIndex = paths[color].last().index;
    paths[color].cutPathIncluding(lastElementIndex);


    var newBlockItem = (RailsTemplate)constructionRailsRes[color].Instance();
    rotateNewRails((Spatial)newBlockItem, paths[color].last().index, lastElementIndex);
    tiles[lastElementIndex].placeItem((Spatial)newBlockItem);

    paths[color].add(paths[color].last().index, lastElementIndex, newBlockItem);
  }

  private int? clickedOnCandidate(int clickedIndex) {
    foreach (int c in pathBuilder.nextCandidates(selected.Value)) {
      if (c == clickedIndex) {
        return c;
      }
    }

    return null;
  }

  private void rotateNewRails(Spatial rails, int from, int to) {
    var diff = to - from;
    if (diff == 1) {
      rails.Rotate(new Vector3(0, 1, 0), Utils.ConvertToRadians(90));
    } else if (diff == -1) {
      rails.Rotate(new Vector3(0, 1, 0), Utils.ConvertToRadians(-90));
    } else if (diff < -1) {
      rails.Rotate(new Vector3(0, 1, 0), Utils.ConvertToRadians(180));
    }
  }

  private void updateLastRail(int updateI, int nextI, PlayColor color) {
    if (!tiles[updateI].hasDepot) {
      paths[color].cutPathIncluding(updateI);
      var fromI = paths[color].last().index;

      var newBlockItem = determineLastBlock(fromI, updateI, nextI, color);
      rotateNewRails((Spatial)newBlockItem, fromI, updateI);

      tiles[updateI].placeItem((Spatial)newBlockItem);
      paths[color].add(fromI, updateI, newBlockItem);
    }
  }

  private RailsTemplate determineLastBlock(int fromI, int currentI, int nextI, PlayColor color) {
    // we're not building next block, so the last block should be construction
    if (currentI == nextI) {
      return (RailsTemplate)constructionRailsRes[color].Instance();
    }

    var diff = nextI - fromI;
    var diffAbs = Math.Abs(diff);

    // straight
    if (diff == 2 || -diff == 2 || diff == 2 * cols || -diff == 2 * cols) {
      return (RailsTemplate)straightRailsRes[color].Instance();
    }

    // right
    if (diffAbs == cols - 1) {
      if (Math.Abs(currentI - fromI) == cols) {
        return (RailsTemplate)rightRailsRes[color].Instance();
      } else if (Math.Abs(currentI - fromI) == 1) {
        return (RailsTemplate)leftRailsRes[color].Instance();
      }
    }

    if (diffAbs == cols + 1) {
      if (Math.Abs(currentI - fromI) == cols) {
        return (RailsTemplate)leftRailsRes[color].Instance();
      } else if (Math.Abs(currentI - fromI) == 1) {
        return (RailsTemplate)rightRailsRes[color].Instance();
      }
    }

    throw new Exception(string.Format(@"This should not happen.
    We didn't detect correct shape of rails when updating last path.
    (fromI: '{0}', currentI: '{1}', nextI: '{2}', color: '{3}'", fromI, currentI, nextI, color));
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
      train.go(paths[train.color].startDepot, paths[train.color].endDepot);
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
