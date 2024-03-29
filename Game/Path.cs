using Godot;
using System.Collections.Generic;

public delegate void PathCompleteChanged(bool complete, PlayColor color);

public class Path {
  public event PathCompleteChanged pathCompleteEvent;

  public PlayColor color { get; }
  public PathElement startDepot { get; set; }
  public PathElement endDepot { get; set; }
  public bool complete { get; private set; }

  public Path(PlayColor color) {
    this.color = color;
  }

  public PathElement last() {
    var last = startDepot;
    while (last.next != null) {
      last = last.next;
    }
    return last;
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
    if (to == endDepot.index) { // build to the end depot => path is complete
      current.next = endDepot;
      completePath(true);
    } else {
      current.next = new PathElement(to, obj);
    }
  }

  public void cutPath(int i) {
    var current = startDepot;
    while (current.index != i && current.next != null) {
      current = current.next;
    }
    if (current.next != null) {
      cutPathFrom(current.next);
      current.next = null;
    }
  }

  public void cutPathIncluding(int i) {
    var current = startDepot;
    while (current.next != null && current.next.index != i) {
      current = current.next;
    }
    if (current.next != null) {
      cutPathFrom(current.next);
      current.next = null;
    }
  }

  private void cutPathFrom(PathElement pathElement) {
    cleanPath(pathElement);
    completePath(false);
  }

  private void completePath(bool complete) {
    this.complete = complete;
    pathCompleteEvent?.Invoke(complete, color);
  }

  // recursively cut the path from given element
  private void cleanPath(PathElement pathElement) {
    // don't clean depot
    if (pathElement.item is DepotTemplate) {
      return;
    }

    // clean itself
    Node pathItem = (Node)pathElement.item;
    pathItem.GetParent().RemoveChild(pathItem);
    pathItem.QueueFree();

    // take next and clean it
    var next = pathElement.next;
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