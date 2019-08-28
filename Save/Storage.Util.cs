using System;
using System.Collections;
using UniRx.Async;
using UnityEngine;

namespace UniSave
{
    public static partial class Storage<T> where T : struct
    {
        #region Load
        
        public static Vector2 LoadVector2(string fileName)
        {
            var data = LoadSync(fileName);
            var vec2 = Vector2.zero;

            vec2.x = (float) data["x"];
            vec2.y = (float) data["y"];

            return vec2;
        }

        public static Vector2Int LoadVector2Int(string fileName)
        {
            var data = LoadSync(fileName);
            var vec2Int = Vector2Int.zero;

            vec2Int.x = (int) data["x"];
            vec2Int.y = (int) data["y"];

            return vec2Int;
        }

        public static Vector3 LoadVector3(string fileName)
        {
            var data = LoadSync(fileName);
            var vec3 = Vector3.zero;

            vec3.x = (float) data["x"];
            vec3.y = (float) data["y"];
            vec3.z = (float) data["z"];

            return vec3;
        }

        public static Vector3Int LoadVector3Int(string fileName)
        {
            var data = LoadSync(fileName);
            var vec3Int = Vector3Int.zero;

            vec3Int.x = (int) data["x"];
            vec3Int.y = (int) data["y"];
            vec3Int.z = (int) data["z"];

            return vec3Int;
        }

        public static Quaternion LoadQuaternion(string fileName)
        {
            var data = LoadSync(fileName);
            var quaternion = Quaternion.identity;

            quaternion.x = (float) data["x"];
            quaternion.y = (float) data["y"];
            quaternion.z = (float) data["z"];
            quaternion.w = (float) data["w"];

            return quaternion;
        }
        
        
        
        #endregion
        
        #region Load Array

        public static Vector3[] LoadVector3Array(string fileName)
        {
            var data = LoadArraySync(fileName);
            var vectors = new Vector3[data.Length];

            for (var i = 0; i < data.Length; i++)
            {
                var dynamic = data[i];
                var vector = vectors[i];

                vector.x = (float) dynamic["x"];
                vector.y = (float) dynamic["y"];
                vector.z = (float) dynamic["z"];

                vectors[i] = vector;
            }

            return vectors;
        }
        
        #endregion

        #region Async Load

        public static async UniTask<Vector2> LoadVector2Async(string fileName)
        {
            var data = await LoadAsync(fileName);
            var vec2 = Vector2.zero;

            vec2.x = (float) data["x"];
            vec2.y = (float) data["y"];

            return vec2;
        }
        
        public static async UniTask<Vector2Int> LoadVector2IntAsync(string fileName)
        {
            var data = await LoadAsync(fileName);
            var vec2 = Vector2Int.zero;

            vec2.x = (int) data["x"];
            vec2.y = (int) data["y"];

            return vec2;
        }
        
        public static async UniTask<Vector3> LoadVector3Async(string fileName)
        {
            var data = await LoadAsync(fileName);
            var vec3 = Vector3.zero;

            vec3.x = (float) data["x"];
            vec3.y = (float) data["y"];
            vec3.z = (float) data["z"];

            return vec3;
        }
        
        public static async UniTask<Vector3Int> LoadVector3IntAsync(string fileName)
        {
            var data = await LoadAsync(fileName);
            var vec3 = Vector3Int.zero;

            vec3.x = (int) data["x"];
            vec3.y = (int) data["y"];
            vec3.z = (int) data["z"];

            return vec3;
        }
        
        public static async UniTask<Quaternion> LoadQuaternionAsync(string fileName)
        {
            var data = await LoadAsync(fileName);
            var rot = Quaternion.identity;

            rot.x = (float) data["x"];
            rot.y = (float) data["y"];
            rot.z = (float) data["z"];
            rot.w = (float) data["w"];

            return rot;
        }

        #endregion
        
        #region Async Array Load
        
        public static async UniTask<Vector3[]> LoadVector3ArrayAsync(string fileName)
        {
            var data = await LoadArrayAsync(fileName);
            var vectors = new Vector3[data.Count];

            for (var i = 0; i < data.Count; i++)
            {
                var dynamic = data[i];
                var vector = vectors[i];

                vector.x = (float) dynamic["x"];
                vector.y = (float) dynamic["y"];
                vector.z = (float) dynamic["z"];

                vectors[i] = vector;
            }

            return vectors;
        }
        
        #endregion
    }

    public readonly struct Load
    {
        
    }
    
    public static class FileName
    {
        public static readonly string Inventory = "gachaResult";
        public static readonly string GotItems = "GotItems";
    }
}