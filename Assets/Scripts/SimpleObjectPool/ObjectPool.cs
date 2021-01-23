using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleObjectPool
{
    [System.Serializable]
    public struct PoolData
    {
        public string     searchKey;
        public int        capacity;
        public GameObject poolingObject;
        public bool       isUI;
    }

    public class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField]
        private Canvas     ownerCanvas = null;
        [SerializeField]
        private PoolData[] poolDatas   = null;

        private Dictionary<string, Queue<GameObject>> poolDic = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            foreach (var poolData in poolDatas)
            {
                var pool = new Queue<GameObject>(poolData.capacity);
                poolDic.Add(poolData.poolingObject.name, pool);

                var root = new GameObject(poolData.poolingObject.name + "s");
                if (poolData.isUI)
                {
                    var rectTransform = root.AddComponent<RectTransform>();
                    rectTransform.SetParent(ownerCanvas.transform);
                    rectTransform.anchorMax = Vector2.zero;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.pivot     = Vector2.zero;
                }
                else
                    root.transform.SetParent(transform);

                root.transform.localPosition = Vector3.zero;
                root.transform.localScale    = Vector3.one;

                for (int i = 0; i < poolData.capacity; i++)
                {
                    var newObject = Instantiate(poolData.poolingObject, root.transform);
                    pool.Enqueue(newObject);
                }
            }
        }

        private IEnumerator Start()
        {
            yield return null;

            foreach (var poolPair in poolDic)
            {
                var pool = poolPair.Value;

                foreach (var poolingObject in pool)
                {
                    poolingObject.SetActive(false);

                    PoolingObject poolingObjectComp = poolingObject.GetComponent<PoolingObject>();
                    if (poolingObjectComp == null)
                        poolingObjectComp = poolingObject.AddComponent<PoolingObject>();

                    poolingObjectComp.Setup(pool);
                }
            }
        }

        public GameObject GetSleepingObject(string searchKey)
        {
            var pool = poolDic[searchKey];

            if (pool.Count == 0)
                return null;

            return pool.Dequeue();
        }
    }
}