using UnityEngine;
using Components.Movement;
using System;

namespace Core.Player   
{
    /// <summary>
    /// 玩家控制器：处理玩家相关的逻辑
    /// </summary>
    [RequireComponent(typeof(MovementComponent))]
    [RequireComponent(typeof(WeaponController))]
    public class PlayerController : MonoBehaviour
    {   
        [Header("组件引用")]
        private MovementComponent _movementComponent;
        private WeaponController _weaponController;
        
        [Header("属性")]
        [SerializeField] private float maxHealth = 10f;
        private float currentHealth;
        private int _currentExp;
        private int _level = 1;
        private int _expToNextLevel = 10;

        private void Awake()
        {
            _movementComponent = GetComponent<MovementComponent>();
            _weaponController = GetComponent<WeaponController>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            Vector2 moveInput = new Vector2(horizontalInput, verticalInput);

            if (_movementComponent != null)
            {
                _movementComponent.SetMovement(moveInput);
            }
            else
            {
                Debug.LogError("MovementComponent未找到!");
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
            // 这里后续需要触发UI更新事件
        }

        private void Die()
        {
            // 这里后续实现死亡逻辑
            gameObject.SetActive(false);
        }

        public void AddExperience(int amount)
        {
            _currentExp += amount;
            if (_currentExp >= _expToNextLevel)
            {
                LevelUp();
            }
            // 这里后续需要触发UI更新事件
        }

        private void LevelUp()
        {
            _level++;
            _currentExp -= _expToNextLevel;
            _expToNextLevel = 10 * _level; // 简单的等级提升公式
            maxHealth += 3; // 每级增加3点生命值上限
            currentHealth = maxHealth; // 升级时回满血
            // 这里后续需要触发升级事件
        }

        // 增加最大生命值的方法
        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
            // 同时恢复生命值到新的最大值
            currentHealth = maxHealth;
            
            // 如果有UI更新事件，在这里触发
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        // 生命值变化事件（可选）
        public event Action<float, float> OnHealthChanged;
    }   
}