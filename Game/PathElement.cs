public class PathElement {
  public int index { get; }
  public PlayObject item { get; }
  public PathElement next { get; set; }

  public PathElement(int index, PlayObject item) {
    this.index = index;
    this.item = item;
  }

  public override string ToString() {
    return string.Format("PathElement[index: '{0}', item: '{1}', next: '{2}']", index, item, next);
  }
}
