

public delegate void ItemClicked(PathBuildElement pathElement);

public interface ClickableItem {
  event ItemClicked objectClicked;

  void select(bool highlightThis = true);
  void cancelSelect();
}
