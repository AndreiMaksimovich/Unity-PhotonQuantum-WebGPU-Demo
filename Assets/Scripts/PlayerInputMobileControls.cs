namespace Amax.QuantumDemo
{
    public class PlayerInputMobileControls : GameSingleton<PlayerInputMobileControls>, IEventBusListener<PlayerInputController.OnPlayerInputModeChanged>
    {
        protected override void AwakeInitialize()
        {
            EventBus.AddListener(this);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(this);
        }

        private void Start()
        {
            UpdateView();
        }

        public void OnEvent(PlayerInputController.OnPlayerInputModeChanged data)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            gameObject.SetActive(PlayerInputController.Instance.Mode == PlayerInputController.EMode.Touchscreen);
        }
    }
}
