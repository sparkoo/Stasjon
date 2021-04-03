

public delegate void ItemClicked(PathElement pathElement);

public interface ClickableItem {
  event ItemClicked objectClicked;
}