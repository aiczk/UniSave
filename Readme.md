# UniSave

UniSave is an asset that allows you to easily save and load values.

  - Supports structure encryption/decryption.
  - The structure array can be preserved.
  - It supports asynchronous loading.
  - Can save most of the structures used in Unity.

### Installation

   - Import UniRx.Async(https://github.com/neuecc/UniRx)
   - Import Utf8Json(https://github.com/neuecc/Utf8Json)

### Intoroduction

Synchronous load/save.
```cs
var saveData = Storage<Vector3>.LoadVector3("FileName");
Storage<Vector3>.Save(Vector3.Zero,"FileName");
```

Synchronous load/save arrays.
```cs
var saveData = Storage<Vector3>.LoadVector3Array("FileName");
Storage<Vector3>.SaveArray(array,"FileName");
```

Asynchronous load/save.
```cs
var saveData = await Storage<Vector3>.LoadVector3Async("FileName");
await Storage<Vector3>.SaveAsync(Vector3.Zero,"FileName");
```

Asynchronous array load/save.
```cs
var saveData = await Storage<Vector3>.LoadVector3ArrayAsync("FileName");
await Storage<Vector3>.SaveArrayAsync(array,"FileName");
```

License
----

MIT