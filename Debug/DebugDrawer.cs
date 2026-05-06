using UnityEngine;

namespace Debug
{
    public class DebugDrawer : MonoBehaviour
    {
        [Header("Debug Gizmo Settings")]
        [SerializeField] private bool arenaWalls;
        [SerializeField] private bool massVelocity;
        [SerializeField] private bool massForce;
        [SerializeField] private bool massAcceleration;
        
        // References
        private ObjectGravityManager _objectGravityManager;
        private ObjectDraggableManager _objectDraggableManager;
        private GameManager _gameManager;
        void Start()
        {
            if (!Application.isEditor) this.enabled = false;
            _objectGravityManager = ObjectGravityManager.Instance;
            _objectDraggableManager = ObjectDraggableManager.Instance;
            _gameManager = GameManager.Instance;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (arenaWalls) DrawWalls();
        }

        private void DrawWalls()
        {
            Gizmos.color = Color.yellow;
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(_gameManager.planetPositionLimit.x, _gameManager.planetPositionLimit.y, -9); // Top right
            corners[1] = new Vector3(-_gameManager.planetPositionLimit.x, _gameManager.planetPositionLimit.y, -9); // Top Left
            corners[2] = new Vector3(_gameManager.planetPositionLimit.x, -_gameManager.planetPositionLimit.y, -9); // bottom Right
            corners[3] = new Vector3(-_gameManager.planetPositionLimit.x, -_gameManager.planetPositionLimit.y, -9); // Bottom Left
            
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[0], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[1], corners[3]);
        }
    }

}
