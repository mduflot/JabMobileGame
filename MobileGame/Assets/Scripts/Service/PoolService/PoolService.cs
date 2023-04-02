﻿using System.Collections.Generic;
using HelperPSR.Pool;
using UnityEngine;

namespace Service
{
    public class PoolService : IPoolService
    {
        private Dictionary<GameObject, Pool<GameObject>> _pools;

        public void CreatePool(GameObject gameObject, int count)
        {
            _pools.Add(gameObject, new Pool<GameObject>(gameObject, count));
        }

        public void RemovePool(GameObject gameObject)
        {
            _pools.Remove(gameObject);
        }

        public GameObject GetFromPool(GameObject gameObject)
        {
            return _pools[gameObject].GetFromPool();
        }

        public void AddToPool(GameObject gameObject)
        {
            _pools[gameObject].AddToPool(gameObject);
        }

        public void AddToPoolLater(GameObject gameObject, float lifeTime)
        {
            _pools[gameObject].AddToPoolLatter(gameObject, lifeTime);
        }
    }
}