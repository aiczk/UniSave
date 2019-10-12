# UniSave

UniSave is an asset that allows you to easily save and load values.

  - Supports Value types and Reference types encryption/decryption.
  - The structure array can be preserved.
  - It supports asynchronous loading.
  - Can save most of the structures used in Unity.
  - Support Mono/IL2CPP.

### Installation

   - Import UniRx.Async(https://github.com/neuecc/UniRx)
   - Import Utf8Json(https://github.com/neuecc/Utf8Json)

### Intoroduction

Synchronous load/save.
```cs
var saveData = Storage<Vector3>.Load("FileName");
Storage<Vector3>.Save(Vector3.Zero,"FileName");
```

Synchronous load/save arrays.
```cs
var saveData = Storage<Vector3>.LoadArray("FileName");
Storage<Vector3>.SaveArray(array,"FileName");
```

Asynchronous load/save.
```cs
var saveData = await Storage<Vector3>.Load("FileName");
await Storage<Vector3>.SaveAsync(Vector3.Zero,"FileName");
```

Asynchronous array load/save.
```cs
var saveData = await Storage<Vector3>.LoadArrayAsync("FileName");
await Storage<Vector3>.SaveArrayAsync(array,"FileName");
```

License
----

MIT