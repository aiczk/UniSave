using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UniSave;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Load = UniSave.Load;

public class StorageTest : MonoBehaviour
{
    private Vector3[] vec3Array = {Vector3.one, Vector3.forward, Vector3.zero, Vector3.up};
    private Vector2[] vec2Array = {Vector2.one, Vector2.right, Vector2.left, Vector2.down};
    private StorageQueue<Vector2> storageQueue = new StorageQueue<Vector2>();
    
    private async void Start()
    {
        //同期保存
        //0.15sec => 0.018sec
        //0.085秒の速度改善
        
        //非同期保存
        //0.15sec => 0.04sec
        //0.11秒の速度改善
        
        //配列同期保存
        //0.30sec => 0.14sec
        //0.16秒の速度改善
        
        //引き出し
        //0.23sec => 0.01sec
        //0.22秒の速度改善
        
        //配列引き出し
        //0.25sec => 0.04sec
        //0.21秒の速度改善
        
        //非同期ロード
        //0.08sec
        //わりとはやい
        
        //非同期配列ロード
        //1.47sec
        //クソ遅い
        
        //average - 0.175sec
        
        var stopWatch = new Stopwatch();
                
        stopWatch.Start();
        Storage<Quaternion>.Save(Quaternion.identity,"FILE");
        stopWatch.Stop();
        Debug.Log($"SaveSync :{stopWatch.Elapsed.ToString()}");
        
        stopWatch.Reset();
        stopWatch.Start();
        await Storage<Quaternion>.SaveAsync(Quaternion.identity,"FILE");
        stopWatch.Stop();
        Debug.Log($"SaveAsync :{stopWatch.Elapsed.ToString()}");
                
        stopWatch.Reset();
        stopWatch.Start();
        Storage<Vector3>.SaveArray(vec3Array,"Array");
        stopWatch.Stop();
        Debug.Log($"SaveArraySync :{stopWatch.Elapsed.ToString()}");
        
        stopWatch.Reset();
        stopWatch.Start();
        await Storage<Vector3>.SaveArrayAsync(vec3Array,"Array");
        stopWatch.Stop();
        Debug.Log($"SaveArrayAsync :{stopWatch.Elapsed.ToString()}");
        
        Add();
        stopWatch.Reset();
        stopWatch.Start();
        storageQueue.Save();
        stopWatch.Stop();
        Debug.Log($"SaveQueueSync :{stopWatch.Elapsed.ToString()}");
        
        Add();
        stopWatch.Reset();
        stopWatch.Start();
        await storageQueue.SaveAsync();
        stopWatch.Stop();
        Debug.Log($"SaveQueueAsync :{stopWatch.Elapsed.ToString()}");
        
        //Storage<Load>.DeleteAll();
    }

    private void Add()
    {
        storageQueue.Add("Q01", Vector2.up);
        storageQueue.Add("Q02", Vector2.one);
        storageQueue.Add("Q03", Vector2.down);
        storageQueue.Add("Q04", vec2Array);
    }
}
