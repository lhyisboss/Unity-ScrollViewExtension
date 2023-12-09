using UnityEngine;

namespace ScrollViewExtension.Scripts.Common
{
    public static class Vector4Extensions
    {
        public static bool GreaterThan(this Vector4 a, Vector4 b)
        {
            return a.magnitude > b.magnitude;
        }

        public static bool LessThan(this Vector4 a, Vector4 b)
        {
            return a.magnitude < b.magnitude;
        }
        
        public static bool LessThanOrEqualWithTolerance(this Vector4 a, Vector4 b, float tolerance)
        {
            return a.magnitude < b.magnitude || Mathf.Abs(a.magnitude - b.magnitude) <= tolerance;
        }

    }
}