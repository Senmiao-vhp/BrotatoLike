using UnityEngine;
using System.Collections.Generic;

namespace Managers.Pool
{
    /// <summary>
    /// 子弹池管理器：管理子弹的创建和回收
    /// </summary>
    public class BulletPoolManager : MonoBehaviour
    {
        public static BulletPoolManager Instance { get; private set; }

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 20;

        private Queue<GameObject> _bulletPool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePool()
        {
            _bulletPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                CreateNewBullet();
            }
        }

        private void CreateNewBullet()
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
        }

        public GameObject GetBullet()
        {
            if (_bulletPool.Count == 0)
            {
                CreateNewBullet();
            }

            GameObject bullet = _bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            _bulletPool.Enqueue(bullet);
        }
    }
}