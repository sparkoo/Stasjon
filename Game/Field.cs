using System.Collections.Generic;

public class Field {
  private int sizeX;
  private int sizeY;

  IList<Block> blocks;
  public Field(int sizeX, int sizeY) {
    this.sizeX = sizeX;
    this.sizeY = sizeY;
    this.blocks = new List<Block>();
  }
}
