using Godot;
using System.Collections.Generic;

public class Game : Node {
  private readonly IList<string> levels = new List<string>() {
    "res://Levels/Level1.tscn",
    "res://Levels/Level2.tscn",
    "res://Levels/Level3.tscn",
    "res://Levels/Level4.tscn",
    "res://Levels/Level5.tscn",
    // "res://Levels/LevelTest.tscn",
  };

  private readonly string winGameScreen = "res://Game/WinGame.tscn";

  private int currentLevel;

  public override void _Ready() {
    startGame();
  }

  public void startGame(int level = 0) {
    currentLevel = level;
    changeLevel(currentLevel);
  }

  public void exitGame() {
    GetTree().Quit();
  }

  private void changeLevel(int level) {
    this.currentLevel = level;
    GetTree().ChangeScene(levels[currentLevel]);
  }

  public void nextLevel() {
    if (currentLevel < levels.Count - 1) {
      changeLevel(currentLevel + 1);
    } else {
      GD.Print("you won the game");
      GetTree().ChangeScene(winGameScreen);
    }
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    if (@event.IsActionPressed("exit")) {
      GD.Print("pressed exit, quitting");
      GetTree().Quit();
    }
  }

}
