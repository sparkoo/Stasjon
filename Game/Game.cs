using Godot;
using System.Collections.Generic;

public class Game : Node {
  private readonly IList<string> levels = new List<string>() {
    "res://Levels/Level1.tscn",
    "res://Levels/Level2.tscn",
    "res://Levels/Level3.tscn",
    "res://Levels/Level4.tscn",
    "res://Levels/Level5.tscn",
  };

  private int currentLevel = 0;

  public override void _Ready() {
    changeLevel(currentLevel);
  }

  public void changeLevel(int level) {
    this.currentLevel = level;
    GetTree().ChangeScene(levels[currentLevel]);
  }

  public void nextLevel() {
    changeLevel(currentLevel + 1);
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    if (@event.IsActionPressed("exit")) {
      GD.Print("pressed exit, quitting");
      GetTree().Quit();
    }
  }

}
