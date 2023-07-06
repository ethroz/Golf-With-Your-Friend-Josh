using System;
using UnityEngine;

public class SimpleBezier<T>
{
    public readonly T[] points;

    public SimpleBezier(T[] points)
    {
        if (points.Length < 2 || points.Length > 4)
        {
            throw new NotSupportedException("Only supports linear, quadratic, and cubic bezier curves");
        }
        this.points = points;
    }
}

// Extension functions are used to allow explicitly defined generic classes while avoiding the use of the dynamic type for better performance.
// Add more as they are needed.
public static class SimpleBezierAt
{
    public static float At(this SimpleBezier<float> b, float t) 
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        float t1 = 1 - t;
        switch(b.points.Length)
        {
            case 2: return t1 * b.points[0] + t * b.points[1];
            case 3: return MathE.Square(t1) * b.points[0] + 2.0f * t1 * t * b.points[1] + MathE.Square(t) * b.points[2];
            case 4: return MathE.Cube(t1) * b.points[0] + 3.0f * MathE.Square(t1) * t * b.points[1] + 3.0f * t1 * MathE.Square(t) * b.points[2] + MathE.Cube(t) * b.points[3];
            default: throw new NotImplementedException();
        }
    }

    public static Vector2 At(this SimpleBezier<Vector2> b, float t) 
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        float t1 = 1 - t;
        switch(b.points.Length)
        {
            case 2: return t1 * b.points[0] + t * b.points[1];
            case 3: return MathE.Square(t1) * b.points[0] + 2.0f * t1 * t * b.points[1] + MathE.Square(t) * b.points[2];
            case 4: return MathE.Cube(t1) * b.points[0] + 3.0f * MathE.Square(t1) * t * b.points[1] + 3.0f * t1 * MathE.Square(t) * b.points[2] + MathE.Cube(t) * b.points[3];
            default: throw new NotImplementedException();
        }
    }

    public static Vector3 At(this SimpleBezier<Vector3> b, float t) 
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        float t1 = 1 - t;
        switch(b.points.Length)
        {
            case 2: return t1 * b.points[0] + t * b.points[1];
            case 3: return MathE.Square(t1) * b.points[0] + 2.0f * t1 * t * b.points[1] + MathE.Square(t) * b.points[2];
            case 4: return MathE.Cube(t1) * b.points[0] + 3.0f * MathE.Square(t1) * t * b.points[1] + 3.0f * t1 * MathE.Square(t) * b.points[2] + MathE.Cube(t) * b.points[3];
            default: throw new NotImplementedException();
        }
    }
}
