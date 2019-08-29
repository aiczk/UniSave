using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace UniSave
{
    [DisallowMultipleComponent]
    public class AutoSave2D : MonoBehaviour
    {
        [SerializeField] private string fileName = default;
        [SerializeField] private bool enableAsync = false;
        
        private async UniTask Start()
        {            
            if (enableAsync)
            {
                if (Storage<Load>.IsFileExist(fileName)) 
                    transform.position = await Storage<Load>.LoadVector3Async(fileName);                
                return;
            }
            
            if (Storage<Load>.IsFileExist(fileName)) 
                transform.position = Storage<Load>.LoadVector3(fileName);

            await Observable.Return(1);
        }
        
        private async UniTask OnApplicationQuit()
        {
            if(fileName.Length == 0 || fileName.Length == 0)
                throw new NullReferenceException("No file name is entered!");
                                    
            if (enableAsync)
            {
                await Storage<Vector3>.SaveAsync(transform.position, fileName);
                return;
            }

            Storage<Vector3>.Save(transform.position, fileName);
        }
    }
}