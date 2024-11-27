using UnityEngine;
using Components.Movement;
using Core.Player;
using Core.Interfaces;
using UnityEngine.Serialization;

namespace Core.Enemy
{
    /// <summary>
    /// 敌人控制器
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class EnemyController : MonoBehaviour, ICharacter
    {
        [Header("Enemy Stats")]
        [SerializeField] private float maxHealth = 3f;
        [SerializeField] private float attackDamage = 1f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private int experienceValue = 1;

        [FormerlySerializedAs("_movementComponent")]
        [Header("References")]
        [SerializeField] private MovementComponent movementComponent;
        [FormerlySerializedAs("_spriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;

        [FormerlySerializedAs("_hitEffectPrefab")]
        [Header("Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [FormerlySerializedAs("_deathEffectPrefab")] [SerializeField] private GameObject deathEffectPrefab;

        private float _currentHealth;
        private float _lastAttackTime;
        private Transform _playerTransform;
        private PlayerController _player;
        private bool _isDead;
        private Color _originalColor;

        private void Awake()
        {
            if (movementComponent == null)
                movementComponent = GetComponent<MovementComponent>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            _originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_isDead) return;
            
            UpdateBehavior();
        }

        private void Initialize()
        {
            _currentHealth = maxHealth;
            _isDead = false;
            
            // 查找玩家
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
                _player = playerObj.GetComponent<PlayerController>();
            }

            // 设置移动速度
            movementComponent.SetSpeed(moveSpeed);
        }

        private void UpdateBehavior()
        {
            if (_playerTransform == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                TryAttack();
                movementComponent.Move(Vector2.zero); // 停止移动
            }
            else
            {
                ChasePlayer();
            }
        }

        private void ChasePlayer()
        {
            if (_playerTransform == null) return;

            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            movementComponent.Move(direction);

            // 更新朝向
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        private void TryAttack()
        {
            if (Time.time - _lastAttackTime >= attackCooldown)
            {
                Attack();
                _lastAttackTime = Time.time;
            }
        }

        private void Attack()
        {
            if (_player != null && Vector2.Distance(transform.position, _player.transform.position) <= attackRange)
            {
                _player.TakeDamage((int)attackDamage);
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            
            StartCoroutine(FlashEffect());
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;

            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            if (_player != null)
            {
                _player.AddExperience(experienceValue);
            }

            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 0.2f);
        }

        private System.Collections.IEnumerator FlashEffect()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = _originalColor;
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        public bool IsDead()
        {
            return _isDead;
        }

        /// <summary>
        /// 设置敌人属性
        /// </summary>
        /// <param name="health">生命值</param>
        /// <param name="damage">攻击力</param>
        /// <param name="speed">移动速度</param>
        public void SetStats(float health, float damage, float speed)
        {
            maxHealth = health;
            _currentHealth = health;
            attackDamage = damage;
            moveSpeed = speed;
            
            // 更新移动组件的速度
            if (movementComponent != null)
            {
                movementComponent.SetSpeed(moveSpeed);
            }
        }
    }
}