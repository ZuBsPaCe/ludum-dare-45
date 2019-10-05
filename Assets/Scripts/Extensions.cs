using UnityEngine;

namespace zs.Assets.Scripts
{
    public static class Extensions
    {
        public static Vector3 with_x(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 with_y(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 with_z(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
