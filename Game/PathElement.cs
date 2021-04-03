using System;

public class PathElement {
  private PlayColor color;
  public int index { get; set; }

  public PathElement(PlayColor color) {
    this.color = color;
  }

  public PathElement(PlayColor color, int index) {
    this.color = color;
  }

  public override string ToString() {
    return String.Format("PathElement[color: '{0}', index: '{1}']", color, index);
  }
}