namespace Core.Interfaces
{
    /// <summary>
    /// 角色基础接口，定义游戏中所有角色（玩家/敌人）的基本行为
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        void TakeDamage(float damage);

        /// <summary>
        /// 获取当前生命值
        /// </summary>
        /// <returns>当前生命值</returns>
        float GetCurrentHealth();

        /// <summary>
        /// 检查是否已死亡
        /// </summary>
        /// <returns>是否死亡</returns>
        bool IsDead();
    }
}