using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPlane{
    public TerrainPlane(Mesh _mesh, ShapeSettings shapeSettings,
        int _meshRes, float _sideLength, int _facesPerSide,
        Vector3 _localUp, int _faceIndex){
        //Reference to the mesh to be edited
        mesh = _mesh;
        shapeGenerator = new();
        shapeGenerator.ShapeSettingsUpdate(shapeSettings);

        //Infos about the entire construct
        currentMeshResolution = _meshRes;
        sideLength = _sideLength;
        facesPerSide = _facesPerSide;

        //infos relevant for this particular mesh
        localUp = _localUp;
        axisRight = new Vector3(localUp.y, localUp.z, localUp.x);
        axisUp = Vector3.Cross(localUp, axisRight);
        faceIndex = _faceIndex;
    }

    Mesh mesh;
    ShapeGenerator shapeGenerator;

    int currentMeshResolution;
    float sideLength;
    int facesPerSide;

    Vector3 localUp;
    Vector3 axisRight;
    Vector3 axisUp;
    int faceIndex;


    public void GeneratePlane(){
        Vector3[] verts = new Vector3[currentMeshResolution * currentMeshResolution];
        int[] tris = new int [currentMeshResolution * currentMeshResolution * 2 * 3];

        int currTriIdx = 0;
        for (int y = 0, i = 0; y < currentMeshResolution; y++){
            for (int x = 0; x < currentMeshResolution; x++, i++){
                Vector2 currPercent = new Vector2(x / (currentMeshResolution - 1f) - 0.5f, y / (currentMeshResolution - 1f) - 0.5f);
                Vector2 offset = offsets()[faceIndex];

                Vector3 basePos = localUp * sideLength / 2 + axisRight * offset.x + axisUp * offset.y; //starting position of the mesh
                Vector3 baseVertPos = basePos + ((axisRight * currPercent.x) + (axisUp * currPercent.y)) * sideLength / facesPerSide; //calculate the position of the vertex to put on the mesh
                Vector3 spherePos = shapeGenerator.transformCubePosToSphere(baseVertPos, true); //normalizing the mesh to be a sphere
                Vector3 noisePos = shapeGenerator.CalculatePointOnPlanet(spherePos);

                verts[i] = noisePos;
                if(x < currentMeshResolution - 1 && y < currentMeshResolution - 1){
                    tris[currTriIdx + 0] = i;
                    tris[currTriIdx + 1] = i + 1;
                    tris[currTriIdx + 2] = i + currentMeshResolution + 1;

                    tris[currTriIdx + 3] = i;
                    tris[currTriIdx + 4] = i + currentMeshResolution + 1;
                    tris[currTriIdx + 5] = i + currentMeshResolution;

                    currTriIdx += 6;
                }
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    private Vector2[] offsets(){
        Vector2[] results = new Vector2[facesPerSide * facesPerSide];

        for (int x = 0; x < facesPerSide; x++){
            for (int y = 0; y < facesPerSide; y++){
                results[x + y * facesPerSide] = new Vector2(
                    x * sideLength / facesPerSide - sideLength / facesPerSide * (.5f * (facesPerSide - 1)),
                    y * sideLength / facesPerSide - sideLength / facesPerSide * (.5f * (facesPerSide - 1))
                );
            }
        }

        return results;
    }
}