using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class DrawBezier : Editor
{
    private void OnSceneViewGUI(SceneView sv)
    {
        BezierCurve bc = target as BezierCurve;
        if (bc.points.Count != bc.tangents.Count) // && bc.points.Count != bc.rotations.Count)
        {
            Debug.LogError("The number of points, tangents and rotations MUST all be equal");
            return;
        }
        else if (bc.points.Count < 2)
        {
            Debug.LogError("A curve requires at least two points");
            return;
        }
        Vector3 offset = bc.transform.position;

        ///////////////////// Handles.MakeBezierPoints

        bc.points[0] = Handles.PositionHandle(bc.points[0] + offset, Quaternion.identity) - offset;
        Handles.SphereHandleCap(0, bc.points[0] + offset, Quaternion.identity, 0.1f, EventType.Repaint);
        bc.tangents[0] = Handles.PositionHandle(bc.tangents[0] + offset, Quaternion.identity) - offset;
        Handles.SphereHandleCap(0, bc.tangents[0] + offset, Quaternion.identity, 0.1f, EventType.Repaint);
        for (int i = 0; i < bc.points.Count - 1; ++i)
        {
            bc.points[i + 1] = Handles.PositionHandle(bc.points[i + 1] + offset, Quaternion.identity) - offset;
            Handles.SphereHandleCap(0, bc.points[i + 1] + offset, Quaternion.identity, 0.1f, EventType.Repaint);
            bc.tangents[i + 1] = Handles.PositionHandle(bc.tangents[i + 1] + offset, Quaternion.identity) - offset;
            Handles.SphereHandleCap(0, bc.tangents[i + 1] + offset, Quaternion.identity, 0.1f, EventType.Repaint);
            Handles.DrawBezier(bc.points[i] + offset, bc.points[i + 1] + offset, bc.tangents[i] + offset, bc.tangents[i + 1] + offset, Color.white, null, 4f);
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneViewGUI;
    }
}
