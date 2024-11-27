using UnityEngine;
using Core.Enemy;
using Managers.Pool;

namespace Core.Combat
{
    /// <summary>
    /// 子弹类
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float damage = 1f;
        [SerializeField] private float lifetime = 5f;

        private void OnEnable()
        {
            // 设置子弹生命周期
            Invoke("ReturnToPool", lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 检查是否碰到敌人
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 对敌人造成伤害
                enemy.TakeDamage(damage);
                
                // 子弹击中后返回对象池
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            // 取消所有待执行的 Invoke 调用
            CancelInvoke();
            
            // 将子弹返回对象池
            BulletPoolManager.Instance.ReturnBullet(gameObject);
        }

        // 设置子弹伤害的方法（可以由 WeaponController 调用）
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }
    }
}