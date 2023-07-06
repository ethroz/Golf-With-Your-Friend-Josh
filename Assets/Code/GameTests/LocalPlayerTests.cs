using System;
using NUnit.Framework;
using UnityEngine;

public class LocalPlayerTests
{
    private static float Distance(Vector3[] points)
    {
        float distance = 0.0f;
        for (int i = 1; i < points.Length; ++i)
        {
            distance += Vector3.Distance(points[i], points[i - 1]);
        }
        return distance;
    }

    [Test]
    public void CreatePredictionNoWallsNoBounces()
    {
        const float maxDistance = 10.0f;
        var points = LocalPlayerScript.CreatePrediction(new(), Vector3.forward, maxDistance, 0);
        Assert.AreEqual(2, points.Length);

        float distance = Distance(points);
        Assert.AreEqual(maxDistance, distance, 1e-6f);
    }

    [Test]
    public void CreatePredictionWallsNoBounces()
    {
        var wall = Resources.Load<GameObject>("Test/Square Wall");
        Assert.NotNull(wall);
        var instantiated = GameObject.Instantiate(wall, new Vector3(), new());

        try
        {
            const float maxDistance = 10.0f;
            var points = LocalPlayerScript.CreatePrediction(new(0.0f, 0.0f, 0.1f), (Vector3.back + Vector3.right).normalized, maxDistance, 0);
            Assert.AreEqual(2, points.Length);

            float distance = Distance(points);
            Assert.AreEqual(2.68700671f, distance, 1e-6f);
        }
        catch (Exception ex)
        {
            GameObject.Destroy(instantiated);
            throw ex;
        }
        GameObject.Destroy(instantiated);
    }

    [Test]
    public void CreatePredictionWallsOneBounce()
    {
        var wall = Resources.Load<GameObject>("Test/Square Wall");
        Assert.NotNull(wall);
        var instantiated = GameObject.Instantiate(wall, new Vector3(), new());

        try
        {
            const float maxDistance = 10.0f;
            var points = LocalPlayerScript.CreatePrediction(new(0.0f, 0.0f, 0.1f), (Vector3.back + Vector3.right).normalized, maxDistance, 1);
            Assert.AreEqual(3, points.Length);

            float distance = Distance(points);
            Assert.AreEqual(2.828433f, distance, 1e-6f);
        }
        catch (Exception ex)
        {
            GameObject.Destroy(instantiated);
            throw ex;
        }
        GameObject.Destroy(instantiated);
    }

    [Test]
    public void CreatePredictionWallsTwoBounces()
    {
        var wall = Resources.Load<GameObject>("Test/Square Wall");
        Assert.NotNull(wall);
        var instantiated = GameObject.Instantiate(wall, new Vector3(), new());

        try
        {
            const float maxDistance = 10.0f;
            var points = LocalPlayerScript.CreatePrediction(new(0.0f, 0.0f, 0.1f), (Vector3.back + Vector3.right).normalized, maxDistance, 2);
            Assert.AreEqual(4, points.Length);

            float distance = Distance(points);
            Assert.AreEqual(8.061050f, distance, 1e-6f);
        }
        catch (Exception ex)
        {
            GameObject.Destroy(instantiated);
            throw ex;
        }
        GameObject.Destroy(instantiated);
    }
}
