using System;

namespace UnityEngine
{
    public static class Extensions
    {
        public static void QuitGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
            Application.Quit();
        }

        public static void AssertHasComponent<T>(this GameObject o) where T : Component
        {
            _ = o.GetComponent<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static void GetComponentOrThrow<T>(this Component c, out T component) where T : Component
        {
            component = c.GetComponent<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static void GetComponentInChildrenOrThrow<T>(this Component c, out T component) where T : Component
        {
            component = c.GetComponentInChildren<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static void GetComponentInParentOrThrow<T>(this Component c, out T component) where T : Component
        {
            component = c.GetComponentInParent<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static T GetComponentOrThrow<T>(this Component c) where T : Component
        {
            return c.GetComponent<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static T GetComponentInChildrenOrThrow<T>(this Component c) where T : Component
        {
            return c.GetComponentInChildren<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static T GetComponentInParentOrThrow<T>(this Component c) where T : Component
        {
            return c.GetComponentInParent<T>() ?? throw new MissingComponentException(typeof(T).ToString());
        }

        public static T[] GetOrThrow<T>(int num) where T : Component
        {
            var objs = Object.FindObjectsOfType<T>();
            if (objs.Length != num)
            {
                throw new NotSupportedException("Expected " + num + " " + typeof(T).ToString());
            }
            return objs;
        }

        public static T GetOrThrow<T>() where T : Component
        {
            return GetOrThrow<T>(1)[0];
        }

        public static void ThrowIfNull(object o)
        {
            if (o == null) throw new NullReferenceException(o.GetType() + "was null");
        }

        public static void AssertTrue(bool b)
        {
            if (!b) throw new ArgumentException(b.GetTypeCode().ToString());
        }
    }

    // Math extensions.
    public static class MathE
    {
        public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }

        public static float Square(float value)
        {
            return value * value;
        }

        public static float Cube(float value)
        {
            return value * value * value;
        }

        public static float ModBetween(float value, float min, float max) {
            return ((value + max) % (max - min)) - max;
        }
    }

    public class Ref<T> where T : struct
    {
        public T Value { get; set; }
    }
}
