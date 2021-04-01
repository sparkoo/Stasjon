using Godot;
using System;

public class Game : Node {
  public override void _Ready() {

  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    if (@event.IsActionPressed("exit")) {
      GD.Print("pressed exit, quitting");
      GetTree().Quit();
    }
  }

}
