public class Path {
  public readonly PlayColor color;
  public readonly PathElement startDepot;
  public readonly PathElement endDepot;

  public Path(PlayColor color, PathElement startDepot, PathElement endDepot) {
    this.color = color;
    this.startDepot = startDepot;
    this.endDepot = endDepot;
  }
}