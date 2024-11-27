using UnityEngine;
using Managers.Pool;
using Core.Enemy;
using Core.Combat;

namespace Core.Player
{
    /// <summary>
    /// 武器控制器：处理玩家武器相关的逻辑
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("武器属性")]
        [SerializeField] private float fireRate = 1f;  // 射击间隔（秒）
        [SerializeField] private GameObject bulletPrefab;  // 子弹预制体
        [SerializeField] private float bulletSpeed = 10f;  // 子弹速度
        [SerializeField] private float detectionRange = 10f;  // 检测范围
        [SerializeField] private float bulletDamage = 1f;  // 新增：子弹伤害
        
        private float _nextFireTime;
        private Transform _playerTransform;
        
        private void Start()
        {
            _playerTransform = transform;
        }

        private void Update()
        {
            if (Time.time >= _nextFireTime)
            {
                // 寻找最近的敌人并射击
                Transform nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    FireAtTarget(nearestEnemy.position);
                    _nextFireTime = Time.time + fireRate;
                }
            }
        }

        /// <summary>
        /// 寻找最近的敌人
        /// </summary>
        private Transform FindNearestEnemy()
        {
            // 获取所有敌人
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            
            Transform nearestEnemy = null;
            float nearestDistance = detectionRange;

            foreach (EnemyController enemy in enemies)
            {
                if (enemy.IsDead()) continue;  // 跳过已死亡的敌人

                float distance = Vector2.Distance(_playerTransform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            return nearestEnemy;
        }

        /// <summary>
        /// 向目标位置发射子弹
        /// </summary>
        private void FireAtTarget(Vector3 targetPosition)
        {
            // 计算射击方向
            Vector2 direction = (targetPosition - _playerTransform.position).normalized;

            // 从对象池获取子弹
            GameObject bullet = BulletPoolManager.Instance.GetBullet();
            if (bullet != null)
            {
                bullet.transform.position = _playerTransform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, 
                    Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                
                // 设置子弹速度
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = direction * bulletSpeed;
                }

                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.SetDamage(bulletDamage);
                }
            }
        }

        // 可选：绘制检测范围（仅在编辑器中可见）
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}