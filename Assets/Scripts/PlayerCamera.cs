using UnityEngine;

namespace Amax.QuantumDemo
{

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : GameSingleton<PlayerCamera>, IEventBusListener<OnLocalPlayerCharacterAdded>
    {

        [SerializeField] private float cameraSpeed = 2.0f;
        public GameObject PlayerGameObject { get; private set; }

        private void Start()
        {
            EventBus.AddListener(this);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(this);
        }

        private void Update()
        {
            if (!PlayerGameObject) return;

            var targetPosition = PlayerGameObject.transform.position;
            targetPosition.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        }

        public void OnEvent(OnLocalPlayerCharacterAdded data)
        {
            PlayerGameObject = data.GameObject;
        }
    }
}