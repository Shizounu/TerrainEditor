using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    SimpleNoiseFilter[] currentNoiseFilters;


    public void ShapeSettingsUpdate(ShapeSettings _newSettings){
        shapeSettings = _newSettings;

        currentNoiseFilters = new SimpleNoiseFilter[shapeSettings.NoiseLayers.Length];
        for (int i = 0; i < currentNoiseFilters.Length; i++){
            currentNoiseFilters[i] = new SimpleNoiseFilter(shapeSettings.NoiseLayers[i].NoiseSettings);
        }
    }



    public Vector3 transformCubePosToSphere(Vector3 _cubePos, bool _fancySphere)
    {
        if (!_fancySphere)
            return (_cubePos).normalized * shapeSettings.PlanetRadius;

        //Source http://mathproofs.blogspot.com/2005/07/mapping-cube-to-sphere.html

        Vector3 localCubePos = _cubePos / shapeSettings.PlanetRadius;

        float x2 = localCubePos.x * localCubePos.x;
        float y2 = localCubePos.y * localCubePos.y;
        float z2 = localCubePos.z * localCubePos.z;

        float xPrime = localCubePos.x * Mathf.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
        float yPrime = localCubePos.y * Mathf.Sqrt(1 - (x2 + z2) / 2 + (x2 * z2) / 3);
        float zPrime = localCubePos.z * Mathf.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);

        
        return new Vector3(xPrime, yPrime, zPrime).normalized * shapeSettings.PlanetRadius;
    }

    public Vector3 CalculatePointOnPlanet(Vector3 _spherePos)
    {
        Vector3 planetPos = _spherePos;
        float elevation = 0f;
        float firstLayerElevation = 0f;

        if(currentNoiseFilters.Length > 0){
            firstLayerElevation = currentNoiseFilters[0].Evalulate(planetPos);
            if(shapeSettings.NoiseLayers[0].Enabled)
                elevation = firstLayerElevation;
        }

        float mask = 0;
        for (int i = 0; i < currentNoiseFilters.Length; i++){
            if(shapeSettings.NoiseLayers[i].Enabled){
                mask = shapeSettings.NoiseLayers[i].UseFirstLayerAsMask ? firstLayerElevation : 1;
                elevation *= currentNoiseFilters[i].Evalulate(_spherePos) * mask;
            }
        }

        return planetPos * (1 + elevation);
    }
}
