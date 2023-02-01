using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Header("Mesh generation settings")]
    public ShapeSettings shapeGenerator;
    [Range(1, 16)] public int PlanesPerSide = 1;
    private int PlanesPerSideSqr => PlanesPerSide * PlanesPerSide;
    [Range(2, 255)] public int PlaneVertexResolution;
    public Material mat;

    public PlanetChunkManager chunkManager;
    private static Vector3[] directions =
    {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.back,
            Vector3.forward
    };
    TerrainPlane[,] terrainPlanes;
    MeshFilter[,] terrainFilters;

    private void InitializeSides(){
        //Ensure lists are set properly
        if(terrainFilters == null || terrainFilters.Length != directions.Length * PlanesPerSideSqr)
            terrainFilters = new MeshFilter[directions.Length, PlanesPerSideSqr];
        terrainPlanes = new TerrainPlane[directions.Length, PlanesPerSideSqr];
        chunkManager.allChunks = new();

        //Generate different faces
        GameObject newFace;
        for (int sideIndex = 0; sideIndex < directions.Length; sideIndex++){
            for (int planeIndex = 0; planeIndex < PlanesPerSideSqr; planeIndex++){
                //Setup terrain filters
                if(terrainFilters[sideIndex, planeIndex] == null){
                    newFace = new GameObject($"TerrainFace_{sideIndex}|{planeIndex}");
                    newFace.transform.SetParent(this.transform);

                    MeshRenderer renderer = newFace.AddComponent<MeshRenderer>();
                    renderer.material = mat;
                    terrainFilters[sideIndex, planeIndex] = newFace.AddComponent<MeshFilter>();

                    Mesh newMesh = new();
                    newMesh.name = $"TerrainFace_{sideIndex}|{planeIndex}";
                    terrainFilters[sideIndex, planeIndex].sharedMesh = newMesh;
                    
                    MeshCollider collider = newFace.AddComponent<MeshCollider>();
                    collider.sharedMesh = newMesh;
                    
                    newFace.layer = 6;
                    chunkManager.allChunks.Add(new PlanetChunkManager.Chunk(collider, renderer));
                    
                }

                //Pass info into terrain plane
                terrainPlanes[sideIndex, planeIndex] = new TerrainPlane(
                    terrainFilters[sideIndex, planeIndex].sharedMesh, shapeGenerator,
                    PlaneVertexResolution, shapeGenerator.PlanetRadius, PlanesPerSide, 
                    directions[sideIndex], planeIndex
                );
            }
        }
    }



    private void GeneratePlanet(){
        for (int side = 0; side < directions.Length; side++){
            for (int plane = 0; plane < PlanesPerSideSqr; plane++){
                terrainPlanes[side, plane].GeneratePlane();
            }
        }
    }

    [ContextMenu("regen")]
    void Regen(){
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        InitializeSides();
        GeneratePlanet();
    }    

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, shapeGenerator.PlanetRadius);
    }
}
