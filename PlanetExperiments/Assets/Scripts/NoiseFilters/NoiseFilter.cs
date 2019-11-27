using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter
{
    NoiseSettings settings;
    Noise noise = new Noise();

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Eval(Vector3 _point)
    {
        float noiseVal = 0;
        float freq = settings.baseRoughness;
        float ampl = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(_point * freq + settings.centre);
            noiseVal += (v + 1) * 0.5f * ampl;
            freq *= settings.roughness;
            ampl *= settings.persistence;
        }

        noiseVal = Mathf.Max(0.0f, noiseVal - settings.minVal);
        return noiseVal * settings.strength;
    }
}
