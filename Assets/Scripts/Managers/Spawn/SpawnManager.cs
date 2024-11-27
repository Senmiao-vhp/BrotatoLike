using UnityEngine;
using Core.Enemy;

namespace Managers.Spawn
{
    /// <summary>
    /// 生成管理器：管理敌人的生成
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject enemyPrefab;          // 敌人预制体
        [SerializeField] private int enemiesPerWave = 5;         // 每波敌人数量
        [SerializeField] private float spawnInterval = 3f;       // 生成间隔
        [SerializeField] private float spawnRadius = 10f;        // 生成半径
        
        [Header("Wave Settings")]
        [SerializeField] private float waveHealthMultiplier = 1.1f;   // 每波生命值倍率
        [SerializeField] private float waveDamageMultiplier = 1.1f;   // 每波伤害倍率
        [SerializeField] private float waveSpeedMultiplier = 1.05f;   // 每波速度倍率

        private float _nextSpawnTime;
        private int _currentWave = 1;
        private Transform _playerTransform;

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            SpawnWave();
            _nextSpawnTime = Time.time + spawnInterval;
        }

        private void Update()
        {
            if (Time.time >= _nextSpawnTime)
            {
                SpawnWave();
                _nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private void SpawnWave()
        {
            if (_playerTransform == null || enemyPrefab == null) return;

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
            }
            _currentWave++;
        }

        private void SpawnEnemy()
        {
            // 在玩家周围随机位置生成敌人
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = _playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnRadius;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // 获取敌人控制器并设置属性
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                // 根据波次提升敌人属性
                float healthMultiplier = Mathf.Pow(waveHealthMultiplier, _currentWave - 1);
                float damageMultiplier = Mathf.Pow(waveDamageMultiplier, _currentWave - 1);
                float speedMultiplier = Mathf.Pow(waveSpeedMultiplier, _currentWave - 1);

                enemyController.SetStats(
                    3 * healthMultiplier,  // 基础生命值 * 倍率
                    1 * damageMultiplier,  // 基础伤害 * 倍率
                    3 * speedMultiplier    // 基础速度 * 倍率
                );
            }
        }

        // 用于外部控制的公共方法
        public void StartSpawning()
        {
            enabled = true;
            _nextSpawnTime = Time.time + spawnInterval;
        }

        public void StopSpawning()
        {
            enabled = false;
        }

        public int GetCurrentWave()
        {
            return _currentWave;
        }
    }
}