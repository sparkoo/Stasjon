public class PathBuildElement {
  public int index { get; }
  public PlayColor color { get; }

  public PathBuildElement(PlayColor color) {
    this.index = -1;
    this.color = color;
  }

  public PathBuildElement(PlayColor color, int index) {
    this.index = index;
    this.color = color;
  }

  public override string ToString() {
    return string.Format("PathBuildElement[color: '{0}', index: '{1}']", color, index);
  }
}