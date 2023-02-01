using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter
{
    NoiseSettings settings;
    SimplexNoise noise;

    public SimpleNoiseFilter(NoiseSettings _settings){
        settings = _settings;
        noise = new SimplexNoise(_settings.NoiseSeed);
    }

    public float Evalulate(Vector3 _point){
        float noiseVal = 0;
        float frequency = settings.Frequency;
        float amplitude = settings.Amplitude;

        for (int i = 0; i < settings.LayerCount; i++){
            float v = noise.Evaluate(settings.NoiseCenter + _point * frequency);
            noiseVal += (v + 1) * 0.5f * amplitude;
            
            frequency *= settings.Roughness;
            amplitude *= settings.Persistence;
        }

        noiseVal = Mathf.Max(0, noiseVal - settings.GroundLevel);

        return noiseVal * settings.Strength;
    }
}
