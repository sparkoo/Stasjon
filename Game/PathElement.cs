public class PathElement {
  public int index { get; }
  public DepotTemplate depot { get; }
  public RailsTemplate rail { get; }
  public PathElement next { get; set; }

  public PathElement(int index, DepotTemplate depot, RailsTemplate rail) {
    this.index = index;
    this.depot = depot;
    this.rail = rail;
  }

  public override string ToString() {
    return string.Format("PathElement[index: '{0}', depot: '{1}', rail: '{2}', next: '{3}']", index, depot, rail, next != null);
  }
}
