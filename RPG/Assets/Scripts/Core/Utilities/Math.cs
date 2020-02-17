using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
    public static class Math
    {
        public static Vector3Int RoundToInt(Vector3 vector)
        {
            return new Vector3Int(
                Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y),
                Mathf.RoundToInt(vector.z));
        }
    }

}