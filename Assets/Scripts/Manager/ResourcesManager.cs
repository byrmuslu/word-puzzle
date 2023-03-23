namespace WordPuzzle.Manager
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ResourcesManager
    {
        public static ResourcesManager Instance { get; private set; } = new ResourcesManager();

        private List<BaseObject> _frequentlyObjectsPrefabs;

        private List<IPoolable> _poolsObject;

        private ResourcesManager()
        {
            _frequentlyObjectsPrefabs = new List<BaseObject>();
            _poolsObject = new List<IPoolable>();
        }

        public T GetObject<T>() where T : BaseObject
        {
            return GetObject<T>(null, "");
        }

        public T GetObject<T>(Transform parent) where T : BaseObject
        {
            return GetObject<T>(parent, "");
        }

        public T GetObject<T>(string variantKey) where T : BaseObject
        {
            return GetObject<T>(null, variantKey);
        }

        public T GetRandomObjectInVariants<T>(Transform parent = null) where T : BaseObject
        {
            List<T> objPrefabs = GetObjectsPrefabs<T>();

            T randomObjPrefab = objPrefabs[UnityEngine.Random.Range(0, objPrefabs.Count)];

            if (_poolsObject.Any(p => p.GetType().Equals(randomObjPrefab.GetType()) && p.VariantKey.Equals(randomObjPrefab.VariantKey)))
            {
                IPoolable poolObj = _poolsObject.Find(p => p is T && p.VariantKey.Equals(randomObjPrefab.VariantKey));
                poolObj.SetParent(parent);
                poolObj.Activate();
                poolObj.AddPool -= OnObjectAddPool;
                poolObj.AddPool += OnObjectAddPool;
                _poolsObject.Remove(poolObj);
                return (T)poolObj;
            }

            if (_frequentlyObjectsPrefabs.Any(f => f is T && f.VariantKey.Equals(randomObjPrefab.VariantKey)))
            {
                BaseObject frequencyObj = _frequentlyObjectsPrefabs.Find(f => f is T && f.VariantKey.Equals(randomObjPrefab.VariantKey));
                frequencyObj.SetParent(parent);
                frequencyObj.Activate();

                if (frequencyObj is IPoolable)
                {
                    (frequencyObj as IPoolable).AddPool += OnObjectAddPool;
                }

                return (T)frequencyObj;
            }

            T obj = MonoBehaviour.Instantiate(randomObjPrefab, parent);

            if (obj is IPoolable)
            {
                (obj as IPoolable).AddPool += OnObjectAddPool;
            }

            return obj;
        }

        public T GetObject<T>(Transform parent, string variantKey) where T : BaseObject
        {
            if (_poolsObject.Any(p => p is T && p.VariantKey.Equals(variantKey)))
            {
                IPoolable poolObj = _poolsObject.Find(p => p is T && p.VariantKey.Equals(variantKey));
                poolObj.SetParent(parent);
                poolObj.Activate();
                _poolsObject.Remove(poolObj);

                if (poolObj is IPoolable)
                {
                    poolObj.AddPool -= OnObjectAddPool;
                    poolObj.AddPool += OnObjectAddPool;
                }

                return (T)poolObj;
            }

            if (_frequentlyObjectsPrefabs.Any(f => f is T && f.VariantKey.Equals(variantKey)))
            {
                BaseObject frequencyObj = _frequentlyObjectsPrefabs.Find(f => f is T && f.VariantKey.Equals(variantKey));
                frequencyObj.SetParent(parent);
                frequencyObj.Activate();

                if (frequencyObj is IPoolable)
                {
                    (frequencyObj as IPoolable).AddPool += OnObjectAddPool;
                }

                return (T)frequencyObj;
            }

            T objectPrefab = GetObjectPrefab<T>(variantKey);

            T obj = MonoBehaviour.Instantiate(objectPrefab, parent);

            if (obj is IPoolable)
            {
                (obj as IPoolable).AddPool += OnObjectAddPool;
            }

            return obj;
        }

        private void OnObjectAddPool(IPoolable poolableObject)
        {
            _poolsObject.Add(poolableObject);
            poolableObject.AddPool -= OnObjectAddPool;
        }

        public void AddFrequentlyObjectPrefab<T>() where T : BaseObject
        {
            T objPrefab = Resources.LoadAll<T>("").FirstOrDefault(o => o is T);

            if (_frequentlyObjectsPrefabs.Contains(objPrefab))
                return;

            _frequentlyObjectsPrefabs.Add(objPrefab);
        }

        public T GetFrequentlyObjectPrefab<T>() where T : BaseObject
        {
            T frequentlyObjectPrefab = (T)_frequentlyObjectsPrefabs.Find(o => o is T);

            if (frequentlyObjectPrefab == null)
                Debug.LogError($"Object not found in frequently objects please add!! Type : {typeof(T).FullName}");

            return frequentlyObjectPrefab;
        }

        public T GetObjectPrefab<T>() where T : BaseObject
        {
            return GetObjectPrefab<T>("");
        }
        public T GetObjectPrefab<T>(string variantKey) where T : BaseObject
        {
            T objPrefab = Resources.LoadAll<T>("").FirstOrDefault(o => o is T && o.VariantKey == variantKey);

            if (objPrefab == null)
                Debug.LogError($"Object not found! Type : {typeof(T).FullName} -- VariantKey : {variantKey}");

            return objPrefab;
        }

        public List<T> GetObjectsPrefabs<T>() where T : MonoBehaviour
        {
            List<T> objsPrefabs = Resources.LoadAll<T>("").ToList();

            if (objsPrefabs == null)
                objsPrefabs = new List<T>();

            return objsPrefabs;
        }

        public List<T> GetScriptableObjects<T>() where T : ScriptableObject
        {
            List<T> objs = Resources.LoadAll<T>("").ToList();

            if (objs == null)
                objs = new List<T>();

            return objs;
        }
    }
}