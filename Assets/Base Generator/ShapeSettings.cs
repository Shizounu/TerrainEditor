using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ShapeSettings", menuName = "PlanetGeneration/ShapeSettings", order = 0)]
public class ShapeSettings : ScriptableObject {
    public float PlanetRadius;
    public bool UseFancySphere;
    public NoiseLayer[] NoiseLayers;

    
    [System.Serializable] public struct NoiseLayer
    {
        public bool Enabled;
        public bool UseFirstLayerAsMask;
        [Space()]
        public NoiseSettings NoiseSettings;
    }
}


[System.Serializable]
public struct NoiseSettings
{
    public int NoiseSeed;
    public float Strength;
    [Range(1, 8)] public int LayerCount;

    public float Frequency;
    public float Persistence;
    public float Amplitude;
    public float Roughness;
    public float GroundLevel;
    
    public Vector3 NoiseCenter;
}
