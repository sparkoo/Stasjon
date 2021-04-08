using Godot;

public class MaterialUtils {
  public static SpatialMaterial setClickableEmission(SpatialMaterial material) {
    material.Emission = Color.Color8(255, 255, 255);
    material.EmissionEnergy = 0.05F;
    material.EmissionOperator = 0;
    material.EmissionOnUv2 = false;
    return material;
  }

  public static void uniqueMaterialWithClickableEmission(MeshInstance mesh, out SpatialMaterial material) {
    // copy the material to be able to set different seeds
    material = (SpatialMaterial)mesh.GetSurfaceMaterial(0).Duplicate(true);
    MaterialUtils.setClickableEmission(material);

    mesh.SetSurfaceMaterial(0, material);
  }
}