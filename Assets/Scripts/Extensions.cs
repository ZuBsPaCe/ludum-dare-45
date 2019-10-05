using UnityEngine;

namespace zs.Assets.Scripts
{
    public static class Extensions
    {
        public static Vector3 with_z(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
