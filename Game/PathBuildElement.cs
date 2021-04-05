using System;

public class PathBuildElement {
  public readonly int index;
  public readonly PlayColor color;

  public PathBuildElement(PlayColor color, int index) {
    this.color = color;
    this.index = index;
  }

  public override string ToString() {
    return String.Format("PathElement[color: '{0}', index: '{1}']", color, index);
  }
}