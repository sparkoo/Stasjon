using System.Collections.Generic;

public class Route {

  IList<Point> points = new List<Point>();



  private class Point {
    private int x;
    private int y;
  }
}