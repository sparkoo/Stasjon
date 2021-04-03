using System;
using Godot;

public partial class RailsStraightBlue : StaticBody, ClickableItem {
  public event ItemClicked objectClicked;

  public void cancelSelect() {
    GD.Print("unselect rail");
  }

  public void select() {
    GD.Print("select rail");
  }

  public override void _Ready() {

  }
}