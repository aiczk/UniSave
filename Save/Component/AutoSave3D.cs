using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace UniSave
{
    [DisallowMultipleComponent]
    public class AutoSave3D : MonoBehaviour
    {
        [SerializeField] private string fileName = default, positionDiff = "position", rotationDiff = "rotation";
        [SerializeField] private bool enableAsync = false;
        
        private async UniTask Awake()
        {
            var pos = $"{fileName}{positionDiff}";
            var rot = $"{fileName}{rotationDiff}";
            
            if (enableAsync)
            {
                transform.position = await Storage<Vector3>.GetOrDefaultAsync(pos);
                transform.rotation = await Storage<Quaternion>.GetOrDefaultAsync(rot);
                return;
            }
            
            transform.position = Storage<Vector3>.GetOrDefault(pos);
            transform.rotation = Storage<Quaternion>.GetOrDefault(rot);
        }
        
        private async UniTask OnApplicationQuit()
        {
            if(fileName.Length == 0 || positionDiff.Length == 0 || rotationDiff.Length == 0)
                throw new NullReferenceException("No file name is entered!");
            
            if(positionDiff == rotationDiff)
                throw new Exception("Duplicate file name.");
            
            var pos = $"{fileName}{positionDiff}";
            var rot = $"{fileName}{rotationDiff}";
            
            if (enableAsync)
            {
                var position = Storage<Vector3>.SaveAsync(transform.position, pos);
                var rotation = Storage<Quaternion>.SaveAsync(transform.rotation, rot);
                
                await UniTask.WhenAll(position, rotation);
                return;
            }

            Storage<Vector3>.Save(transform.position, pos);
            Storage<Quaternion>.Save(transform.rotation, rot);
        }
    }
}