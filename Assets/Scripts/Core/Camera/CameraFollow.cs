using UnityEngine;

namespace Core.Camera
{
    /// <summary>
    /// 相机跟随：处理相机跟随玩家的相关逻辑
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 _offset;

        private void Start()
        {
            if (target != null)
            {
                _offset = transform.position - target.position;
            }
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.position + _offset;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}