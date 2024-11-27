using UnityEngine;
using System;
using Core.Player;

namespace Managers.Game
{
    public class ProgressionManager : MonoBehaviour
    {
        internal static ProgressionManager Instance { get; private set; }

        [Header("升级设置")]
        [SerializeField] private int baseExperienceRequired;  // 基础升级经验需求
        [SerializeField] private float healthIncreasePerLevel;  // 每级生命值增加量

        private int _currentLevel = 1;
        private int _currentExperience;
        private int _experienceToNextLevel;

        public event Action<int> OnLevelUp;  // 升级事件
        public event Action<int, int> OnExperienceGained;  // 经验值获得事件

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                baseExperienceRequired = 10;
                healthIncreasePerLevel = 3f;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _experienceToNextLevel = baseExperienceRequired;
        }

        // 获取当前等级
        public int GetCurrentLevel()
        {
            return _currentLevel;
        }

        // 获取当前经验值
        public int GetCurrentExperience()
        {
            return _currentExperience;
        }

        // 获取升级所需经验值
        public int GetExperienceToNextLevel()
        {
            return _experienceToNextLevel;
        }

        // 添加经验值
        public void AddExperience(int amount)
        {
            _currentExperience += amount;
            OnExperienceGained?.Invoke(_currentExperience, _experienceToNextLevel);

            // 检查是否可以升级
            while (_currentExperience >= _experienceToNextLevel)
            {
                LevelUp();
            }
        }

        // 升级处理
        private void LevelUp()
        {
            _currentExperience -= _experienceToNextLevel;
            _currentLevel++;
            
            // 更新下一级所需经验值（保持为固定值10）
            _experienceToNextLevel = baseExperienceRequired;

            // 增加玩家属性
            var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (player != null)
            {
                player.IncreaseMaxHealth(healthIncreasePerLevel);
            }

            // 触发升级事件
            OnLevelUp?.Invoke(_currentLevel);
        }

        // 重置进度
        public void ResetProgression()
        {
            _currentLevel = 1;
            _currentExperience = 0;
            _experienceToNextLevel = baseExperienceRequired;
        }
    }
}