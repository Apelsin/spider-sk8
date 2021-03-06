﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }
    [Serializable]
    public class Vector3Event : UnityEvent<Vector3> { }
    // Extension method, call for any object, eg "if (x.IsNumeric())..."
    public static bool IsNumeric(this object x)
    {
        return (x == null ? false : IsNumeric(x.GetType()));
    }

    // Method where you know the type of the object
    public static bool IsNumeric(Type type)
    {
        return IsNumeric(type, Type.GetTypeCode(type));
    }

    // Method where you know the type and the type code of the object
    public static bool IsNumeric(Type type, TypeCode typeCode)
    {
        return typeCode == TypeCode.Decimal ||
            (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char);
    }

    public static IEnumerable<string> EnumerateProperties<T>(this T @object)
    {
        var stuff = new Dictionary<string, object>();
        foreach (var prop in @object.GetType().GetProperties())
        {
            stuff[prop.Name] = prop.GetValue(@object, null);
        }
        foreach (var field in @object.GetType().GetFields())
        {
            stuff[field.Name] = field.GetValue(@object);
        }
        foreach (var e in stuff)
        {
            object value;
            if (e.Value.IsNumeric())
                value = $"{e.Value:0.00}";
            else
                value = e.Value;
            yield return $"{e.Key}: {value}";
        }
    }

    public static string DebugProperties<T>(this T @object)
    {
        return string.Join("\n", @object.EnumerateProperties().ToArray());
    }
}
