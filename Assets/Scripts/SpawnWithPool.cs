using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Game.Stats;

namespace Piotr.SpawnWithPool
{
    public class SpawnWithPool
    {
        public List<GameObject> toSpawn = new List<GameObject>();

        // Collection checks will throw errors if we try to release an item that is already in the pool.
        private readonly bool collectionChecks = true;
        private readonly int maxPoolSize = 10;
        private int lastSpawn = 0;

        private Transform elementsContainer;       
        private List<GameObject> poolElements = new List<GameObject>();

        private IObjectPool<GameObject> m_Pool;

        public IObjectPool<GameObject> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    PlayerStats.onGameOver += ReleaseAllFromPool;

                    elementsContainer = new GameObject().GetComponent<Transform>();
                    elementsContainer.name = "poolContainer";
                    m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                }
                return m_Pool;
            }
        }

        #region poolOperations
        private GameObject CreatePooledItem()
        {
            if (lastSpawn >= toSpawn.Count - 1)
            {
                lastSpawn = 0;
            }
            else
            {
                lastSpawn++;
            }

            var poolItem = GameObject.Instantiate(toSpawn[lastSpawn], elementsContainer);
            if (poolItem.GetComponent<IReturningToPool>() != null)
            {
                poolItem.GetComponent<IReturningToPool>().OnInitReturningToPool(ReleaseFromPool);
            }

            poolElements.Add(poolItem);

            return poolItem;
        }

        private void OnReturnedToPool(GameObject system)
        {         
            system.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(GameObject system)
        {
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        private void OnDestroyPoolObject(GameObject system)
        {
            GameObject.Destroy(system.gameObject);
        }
        #endregion

        public void ReleaseFromPool(GameObject released)
        {
            released.transform.parent = elementsContainer;
            Pool.Release(released);
        }

        public void ReleaseAllFromPool(bool isGameOver)
        {
            if (isGameOver)
            {
                foreach (GameObject element in poolElements)
                {
                    if (element.activeInHierarchy)
                    {
                        Pool.Release(element);
                    }
                }
            }
        }
    }
}

public interface IReturningToPool
{
    void OnInitReturningToPool(Action<GameObject> objectForPoolRelease);
}