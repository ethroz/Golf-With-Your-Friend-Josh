using NUnit.Framework;
using UnityEngine;

public class SimpleBezierTests
{
    [Test]
    public void LinearBezierFloat()
    {
        SimpleBezier<float> bezier = new(new float[]{0.0f, 1.0f});
        Assert.AreEqual(0.0f,  bezier.At( 0.0f ));
        Assert.AreEqual(0.25f, bezier.At( 0.25f));
        Assert.AreEqual(0.5f,  bezier.At( 0.5f ));
        Assert.AreEqual(0.75f, bezier.At( 0.75f));
        Assert.AreEqual(1.0f,  bezier.At( 1.0f ));
        Assert.AreEqual(0.0f,  bezier.At(-1.0f ));
        Assert.AreEqual(1.0f,  bezier.At( 2.0f ));
    }

    [Test]
    public void LinearBezierVector()
    {
        SimpleBezier<Vector2> bezier = new(new Vector2[]{new(0.0f, 0.0f), new(1.0f, 2.0f)});
        Assert.AreEqual(new Vector2(0.0f,  0.0f), bezier.At( 0.0f ));
        Assert.AreEqual(new Vector2(0.25f, 0.5f), bezier.At( 0.25f));
        Assert.AreEqual(new Vector2(0.5f,  1.0f), bezier.At( 0.5f ));
        Assert.AreEqual(new Vector2(0.75f, 1.5f), bezier.At( 0.75f));
        Assert.AreEqual(new Vector2(1.0f,  2.0f), bezier.At( 1.0f ));
        Assert.AreEqual(new Vector2(0.0f,  0.0f), bezier.At(-1.0f ));
        Assert.AreEqual(new Vector2(1.0f,  2.0f), bezier.At( 2.0f ));
    }

    [Test]
    public void QuadraticBezierFloat()
    {
        SimpleBezier<float> bezier = new(new float[]{0.0f, 0.0f, 1.0f});
        Assert.AreEqual(0.0f,    bezier.At( 0.0f ));
        Assert.AreEqual(0.0625f, bezier.At( 0.25f));
        Assert.AreEqual(0.25f,   bezier.At( 0.5f ));
        Assert.AreEqual(0.5625f, bezier.At( 0.75f));
        Assert.AreEqual(1.0f,    bezier.At( 1.0f ));
        Assert.AreEqual(0.0f,    bezier.At(-1.0f ));
        Assert.AreEqual(1.0f,    bezier.At( 2.0f ));
    }

    [Test]
    public void QuadraticBezierVector()
    {
        SimpleBezier<Vector2> bezier = new(new Vector2[]{new(0.0f, 0.0f), new(0.0f, 0.0f), new(1.0f, 2.0f)});
        Assert.AreEqual(new Vector2(0.0f,    0.0f  ), bezier.At( 0.0f ));
        Assert.AreEqual(new Vector2(0.0625f, 0.125f), bezier.At( 0.25f));
        Assert.AreEqual(new Vector2(0.25f,   0.5f  ), bezier.At( 0.5f ));
        Assert.AreEqual(new Vector2(0.5625f, 1.125f), bezier.At( 0.75f));
        Assert.AreEqual(new Vector2(1.0f,    2.0f  ), bezier.At( 1.0f ));
        Assert.AreEqual(new Vector2(0.0f,    0.0f  ), bezier.At(-1.0f ));
        Assert.AreEqual(new Vector2(1.0f,    2.0f  ), bezier.At( 2.0f ));
    }

    [Test]
    public void CubicBezierFloat()
    {
        SimpleBezier<float> bezier = new(new float[]{0.0f, 0.0f, 1.0f, 1.0f});
        Assert.AreEqual(0.0f,     bezier.At( 0.0f ));
        Assert.AreEqual(0.15625f, bezier.At( 0.25f));
        Assert.AreEqual(0.5f,     bezier.At( 0.5f ));
        Assert.AreEqual(0.84375f, bezier.At( 0.75f));
        Assert.AreEqual(1.0f,     bezier.At( 1.0f ));
        Assert.AreEqual(0.0f,     bezier.At(-1.0f ));
        Assert.AreEqual(1.0f,     bezier.At( 2.0f ));
    }

    [Test]
    public void CubicBezierVector()
    {
        SimpleBezier<Vector2> bezier = new(new Vector2[]{new(0.0f, 0.0f), new(0.0f, 0.0f), new(1.0f, 2.0f), new(1.0f, 2.0f)});
        Assert.AreEqual(new Vector2(0.0f,     0.0f   ), bezier.At( 0.0f ));
        Assert.AreEqual(new Vector2(0.15625f, 0.3125f), bezier.At( 0.25f));
        Assert.AreEqual(new Vector2(0.5f,     1.0f   ), bezier.At( 0.5f ));
        Assert.AreEqual(new Vector2(0.84375f, 1.6875f), bezier.At( 0.75f));
        Assert.AreEqual(new Vector2(1.0f,     2.0f   ), bezier.At( 1.0f ));
        Assert.AreEqual(new Vector2(0.0f,     0.0f   ), bezier.At(-1.0f ));
        Assert.AreEqual(new Vector2(1.0f,     2.0f   ), bezier.At( 2.0f ));
    }
}
