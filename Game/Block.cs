public class Block {
  private Field field;
  private int x;
  private int y;
  private GroundBlock ground;
  private RailTemplate rail { get; set; }
  private DepotTemplate depot { get; set; }

  public Block(Field field, int x, int y, GroundBlock ground) {
    this.x = x;
    this.y = y;
    this.field = field;
    this.ground = ground;
  }
}
