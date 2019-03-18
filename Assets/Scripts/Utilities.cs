using System;
using UnityEngine;

public static class Utilities
{
    public static float NormDist(float x, float sigma)
    {
        float m = 1f / (sigma * Mathf.Sqrt(2f * Mathf.PI));
        float s = 2f * sigma * sigma;
        return m * Mathf.Exp(-Mathf.Pow(x, 2) / s);
    }

    public static float NormDist(float x)
    {
        float m = 1f / Mathf.Sqrt(2f * Mathf.PI);
        return m * Mathf.Exp(-Mathf.Pow(x, 2) / 2f);
    }

    public static float NormDistFast(float x)
    {
        return Mathf.Exp(-Mathf.Pow(x, 2) / 2f);
    }

    public static Func<float, float> Curry2x1(Func<float, float, float, float> f, float a, float b)
    {
        return x => f(x, a, b);
    }

    public static Func<float, float> SmoothFunc(Func<float, float> f, float knee, float n)
    {
        return x =>
        {
            var y = 0f;
            var sum = 0f;
            for (int i = 0; i < n; i++)
            {
                var t = i / (float)(n - 1);
                var d = 2 * t - 1;
                var w = NormDistFast(3 * d);
                var fx = f(x + d * knee);
                //Debug.Log(fx);
                y += fx * w;
                sum += w;
            }
            y /= sum;
            return y;
        };
    }
}