using UnityEngine;

namespace Components.Movement
{
    /// <summary>
    /// 移动组件：处理角色的移动逻辑
    /// </summary>
    public class MovementComponent : MonoBehaviour
    {
        [Header("移动参数")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _maxX = 8f;  // 水平移动边界
        [SerializeField] private float _maxY = 4f;  // 垂直移动边界

        private Vector2 _movement;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 设置移动速度
        /// </summary>
        public void SetSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        /// <summary>
        /// 设置移动输入（用于玩家控制）
        /// </summary>
        public void SetMovement(Vector2 moveInput)
        {
            _movement = moveInput.normalized;
        }

        /// <summary>
        /// 直接移动（用于AI控制）
        /// </summary>
        public void Move(Vector2 direction)
        {
            SetMovement(direction);
        }

        private void FixedUpdate()
        {
            if (_movement != Vector2.zero)
            {
                Vector3 currentPos = transform.position;
                Vector3 newPosition = currentPos + new Vector3(_movement.x, _movement.y, 0) * (_moveSpeed * Time.fixedDeltaTime);
                
                // 限制位置
                newPosition.x = Mathf.Clamp(newPosition.x, -_maxX, _maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, -_maxY, _maxY);
                
                transform.position = newPosition;
            }
        }
    }
}