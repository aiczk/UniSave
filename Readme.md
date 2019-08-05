# UniSave

UniSave is an asset that allows you to easily save and load values.

  - Supports structure encryption/decryption.
  - The structure array can be preserved.
  - It supports asynchronous loading.
  - Can save most of the structures used in Unity.

### Installation

   - Import UniRx

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

### Todos

 - The encrypted string contains a newline character ( \n) that causes the data to disappear due to incorrect decryption.

License
----

MIT