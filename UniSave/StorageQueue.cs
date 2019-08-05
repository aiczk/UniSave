using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Storage
{
    public class StorageQueue<T> where T : struct
    {
        private Dictionary<string, T> saveDictionary;
        private Dictionary<string, IReadOnlyList<T>> saveArrayDictionary;
        
        public StorageQueue()
        {
            saveDictionary = new Dictionary<string, T>();
            saveArrayDictionary = new Dictionary<string, IReadOnlyList<T>>();
        }
        
        #region Add        
        public void Add(string fileName, T type)
        {
            if(saveDictionary.ContainsKey(fileName))
                return;

            saveDictionary.Add(fileName, type);
        }
        
        public void Add(string fileName, IReadOnlyList<T> type)
        {
            if(saveArrayDictionary.ContainsKey(fileName))
                return;

            saveArrayDictionary.Add(fileName, type);
        }
        #endregion
        
        #region Save
        public void Save(bool append = false)
        {
            if(saveDictionary.Count == 0)
                return;
            
            GetValue(saveDictionary, out var names, out var types);
            
            for (var i = 0; i < saveDictionary.Count; i++)
            {
                var name = names[i];
                var type = types[i];
                
                Storage<T>.Save(type, name, append);
            }

            if (saveArrayDictionary.Count != 0)
                ArraySaveSync(append);
            
            saveDictionary.Clear();
        }

        public async UniTask SaveAsync(bool append = false)
        {
            if(saveDictionary.Count == 0)
                return;

            GetValue(saveDictionary, out var names, out var types);
            
            for (var i = 0; i < saveDictionary.Count; i++)
            {
                var name = names[i];
                var type = types[i];

                await Storage<T>.SaveAsync(type, name, append);
            }

            if (saveArrayDictionary.Count != 0)
                await ArraySaveAsync(append);
            
            saveDictionary.Clear();
        }
        #endregion

        #region Utility
        
        public void Cancel()
        {
            saveDictionary.Clear();
            saveArrayDictionary.Clear();
        }
        
        private void ArraySaveSync(bool append = false)
        {
             if(saveArrayDictionary.Count == 0)
                 return;

             var keys = saveArrayDictionary.Keys.ToArray();
             var values = saveArrayDictionary.Values.ToArray();

             for (var i = 0; i < keys.Length; i++)
             {
                 var name = keys[i];
                 var type = values[i];

                 Storage<T>.SaveArray(type, name, append);
             }
             
             saveArrayDictionary.Clear();
        }
        
        private async UniTask ArraySaveAsync(bool append = false)
        {
            var keys = saveArrayDictionary.Keys.ToArray();
            var values = saveArrayDictionary.Values.ToArray();

            for (var i = 0; i < keys.Length; i++)
            {
                var name = keys[i];
                var type = values[i];

                await Storage<T>.SaveArrayAsync(type, name, append);
            }
             
            saveArrayDictionary.Clear();
        }

        private void GetValue(Dictionary<string,T> dic,out string[] keys, out T[] types)
        {
            keys = dic.Keys.ToArray();
            types = dic.Values.ToArray();
        }
        #endregion
    }
}