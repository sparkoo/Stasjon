using Godot;
using System.Collections.Generic;

public class Path {
  public PlayColor color { get; }
  public PathElement startDepot { get; set; }
  public PathElement endDepot { get; set; }

  public Path(PlayColor color) {
    this.color = color;
  }

  public void add(int from, int to, PlayObject obj) {
    var current = startDepot;
    while (current.next != null && current.index != from) {
      current = current.next;
    }
    GD.Print(string.Format("building on {0}", current));
    if (current.next != null) {
      cleanPath(current.next);
    }
    current.next = new PathElement(to, obj);
  }

  private void cleanPath(PathElement pathElement) {
    var next = pathElement.next;
    pathElement.item.QueueFree();
    if (next != null) {
      cleanPath(next);
    }
  }

  public override string ToString() {
    IList<int> p = new List<int>();
    var current = startDepot;
    p.Add(current.index);
    while (current.next != null) {
      current = current.next;
      p.Add(current.index);
    }
    return string.Format("Path[color: '{0}', startDepot: '{1}', endDepot: '{2}', path: '{3}']", color, startDepot, endDepot, string.Join(", ", p));
  }
}