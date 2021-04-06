public class Path {
  public PlayColor color { get; }
  public PathElement startDepot { get; set; }
  public PathElement endDepot { get; set; }

  public Path(PlayColor color) {
    this.color = color;
  }

  public override string ToString() {
    return string.Format("Path[color: '{0}', startDepot: '{1}', endDepot: '{2}']", color, startDepot, endDepot);
  }
}