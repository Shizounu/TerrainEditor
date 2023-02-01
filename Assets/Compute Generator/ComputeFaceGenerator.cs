using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeFaceGenerator{
    public ComputeFaceGenerator(Mesh _mesh, ShapeSettings _shapeSettings, ComputeShader _vertexCompute, int _MeshResolution, int _MeshIndex, int _PlanesPerSide, Vector3 _localUp){

        //References for generation
        mesh = _mesh;
        shapeSettings = _shapeSettings;
        vertexCompute = _vertexCompute;

        //Infos
        MeshResolution = _MeshResolution;
        MeshIndex = _MeshIndex;
        PlanesPerSide = _PlanesPerSide;

        localUp = _localUp;
        axisRight = new Vector3(localUp.y, localUp.z, localUp.x);
        axisUp = Vector3.Cross(localUp, axisRight);
    }

    #region Pregen variables
    //References
    private Mesh mesh;
    private ShapeSettings shapeSettings;
    private ComputeShader vertexCompute;
    private ComputeBuffer positionsBuffer;
    private ComputeBuffer noiseSettingsBuffer;

    //Calculated Stuff
    private int MeshResolution;
    private int MeshIndex;
    private int PlanesPerSide;

    Vector3 localUp;
    Vector3 axisRight;
    Vector3 axisUp;
    #endregion
   
   #region Compute

    #region IDs
    public readonly int ID_LocalUp = Shader.PropertyToID("VertexCompute_localUp");
    public readonly int ID_AxisRight = Shader.PropertyToID("VertexCompute_axisRight");
    public readonly int ID_AxisUp = Shader.PropertyToID("VertexCompute_axisUp");

    public readonly int ID_SideLength = Shader.PropertyToID("VertexCompute_sideLength");
    public readonly int ID_Offset = Shader.PropertyToID("VertexCompute_offset");

    public readonly int ID_Resolution = Shader.PropertyToID("VertexCompute_Resolution");
    public readonly int ID_FacesPerSide = Shader.PropertyToID("VertexCompute_facesPerSide");

    public readonly int ID_PositionsBuffer = Shader.PropertyToID("VertexCompute_Positions");
    public readonly int ID_NoiseSettingsBuffer = Shader.PropertyToID("VertexCompute_NoiseSettings");
    public readonly int ID_NoiseSettingsLength = Shader.PropertyToID("VertexCompute_NoiseSettingsLength");
    #endregion

    public void computeVerticies(){
        vertexCompute.SetVector(ID_LocalUp, localUp);
        vertexCompute.SetVector(ID_AxisRight, axisRight);
        vertexCompute.SetVector(ID_AxisUp, axisUp);

        vertexCompute.SetFloat(ID_SideLength, shapeSettings.PlanetRadius);
        vertexCompute.SetVector(ID_Offset, offsets()[MeshIndex]);
        
        vertexCompute.SetInt(ID_Resolution, MeshResolution);
        vertexCompute.SetInt(ID_FacesPerSide, PlanesPerSide);

        

        //prepare buffer
        int noiseLayerSize = sizeof(float) * 6 + sizeof(int) * 2 + sizeof(float)*3 * 1;
        noiseSettingsBuffer = new ComputeBuffer(shapeSettings.NoiseLayers.Length, noiseLayerSize);
        //prepare buffer data
        List<NoiseSettings> settings = new();
        for (int i = 0; i < shapeSettings.NoiseLayers.Length; i++){
            settings.Add(shapeSettings.NoiseLayers[i].NoiseSettings);
        }
        noiseSettingsBuffer.SetData<NoiseSettings>(settings);
        vertexCompute.SetBuffer(0, ID_NoiseSettingsBuffer, noiseSettingsBuffer);
        
        vertexCompute.SetInt(ID_NoiseSettingsLength, shapeSettings.NoiseLayers.Length);
        
        
        
        positionsBuffer = new ComputeBuffer(MeshResolution * MeshResolution, sizeof(float) * 3);
        vertexCompute.SetBuffer(0, ID_PositionsBuffer, positionsBuffer);

        int groupCount = Mathf.CeilToInt(MeshResolution / 8f);

        vertexCompute.Dispatch(0, groupCount, groupCount, 1);

    }
   #endregion

    public void GeneratePlane(){
        Vector3[] verts = new Vector3[MeshResolution * MeshResolution];
        int[] tris = new int [MeshResolution * MeshResolution * 2 * 3];
        
        computeVerticies();
        
        //Handles assigning the Tri indicies
        int currTriIdx = 0;
        for (int y = 0, i = 0; y < MeshResolution; y++){
            for (int x = 0; x < MeshResolution; x++, i++){
                if(x < MeshResolution - 1 && y < MeshResolution - 1){
                    tris[currTriIdx + 0] = i;
                    tris[currTriIdx + 1] = i + 1;
                    tris[currTriIdx + 2] = i + MeshResolution + 1;

                    tris[currTriIdx + 3] = i;
                    tris[currTriIdx + 4] = i + MeshResolution + 1;
                    tris[currTriIdx + 5] = i + MeshResolution;

                    currTriIdx += 6;
                }
            }
        }


        mesh.Clear();
        Vector3[] vertexPositions = new Vector3[MeshResolution * MeshResolution];
        positionsBuffer.GetData(vertexPositions);
        mesh.vertices = vertexPositions;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        positionsBuffer.Release(); //free up buffer memory
        positionsBuffer = null;

        noiseSettingsBuffer.Release();
        noiseSettingsBuffer = null;


    }

    private Vector2[] offsets(){
        Vector2[] results = new Vector2[PlanesPerSide * PlanesPerSide];

        for (int x = 0; x < PlanesPerSide; x++){
            for (int y = 0; y < PlanesPerSide; y++){
                results[x + y * PlanesPerSide] = new Vector2(
                    x * shapeSettings.PlanetRadius / PlanesPerSide - shapeSettings.PlanetRadius / PlanesPerSide * (.5f * (PlanesPerSide - 1)),
                    y * shapeSettings.PlanetRadius / PlanesPerSide - shapeSettings.PlanetRadius / PlanesPerSide * (.5f * (PlanesPerSide - 1))
                );
            }
        }

        return results;
    }
}
