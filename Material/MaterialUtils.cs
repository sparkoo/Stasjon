using Godot;

public class MaterialUtils {
  public static SpatialMaterial setClickableEmission(SpatialMaterial material) {
    material.Emission = Color.Color8(255, 255, 255);
    material.EmissionEnergy = 0.02F;
    material.EmissionOperator = 0;
    material.EmissionOnUv2 = false;
    return material;
  }
}