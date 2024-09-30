using UnityEngine;

namespace Amax.QuantumDemo
{

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : GameSingleton<PlayerCamera>
    {

        [SerializeField] private float cameraSpeed = 2.0f;
        public GameObject PlayerGameObject { get; set; }

        private void Update()
        {
            if (!PlayerGameObject) return;

            var targetPosition = PlayerGameObject.transform.position;
            targetPosition.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        }
    }
}