using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Storage
{
    [DisallowMultipleComponent]
    public class AutoSave : MonoBehaviour
    {
        [SerializeField] private string fileName = default, positionDiff = "position", rotationDiff = "rotation";
        [SerializeField] private bool enableAsync = false;
        
        private async UniTask Awake()
        {
            var pos = $"{fileName}{positionDiff}";
            var rot = $"{fileName}{rotationDiff}";
            
            if (enableAsync)
            {
                if (Storage<Load>.IsFileExist(pos)) 
                    transform.position = await Storage<Load>.LoadVector3Async(pos);

                if (Storage<Load>.IsFileExist(rot))
                    transform.rotation = await Storage<Load>.LoadQuaternionAsync(rot);
                
                return;
            }
            
            if (Storage<Load>.IsFileExist(pos)) 
                transform.position = Storage<Load>.LoadVector3(pos);

            if (Storage<Load>.IsFileExist(rot))
                transform.rotation = Storage<Load>.LoadQuaternion(rot);
        }
        
        private async UniTask OnApplicationQuit()
        {
            if(fileName.Length == 0 || string.IsNullOrWhiteSpace(fileName) || positionDiff.Length == 0 || rotationDiff.Length == 0)
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