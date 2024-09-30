using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Input;

namespace Amax.QuantumDemo
{

    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : GameSingleton<PlayerInputController>
    {
        public Vector2 Direction { get; private set; }
        public Vector2 PointerPosition { get; private set; }

        private EMode _mode = EMode.KeyboardAndMouse;
        public EMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                OnPlayerInputModeChanged.Raise(value);
            }
        }

        public bool ButtonShoot { get; private set; }
        public bool ButtonBoost { get; private set; }

        private GameObject _playerGameObject;

        private PlayerInput _playerInput;
        private PlayerInput PlayerInput
        {
            get
            {
                _playerInput ??= GetComponent<PlayerInput>();
                return _playerInput;
            }
        }

        public GameObject PlayerGameObject
        {
            get => _playerGameObject;
            set => _playerGameObject = value;
        }

        private void Start()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
            Mode = RuntimePlatformUtil.IsMobile() ? EMode.Touchscreen : EMode.KeyboardAndMouse;
        }

        private void PollInput(CallbackPollInput callback)
        {
            if (!PlayerGameObject) return;
            
            var direction = Vector2.zero;

            // Mouse & Touchscreen
            if (Mode is EMode.KeyboardAndMouse)
            {
                if (PointerPosition == Vector2.zero) goto return_result;

                var screenPosition = Camera.main.WorldToScreenPoint(PlayerGameObject.transform.position);
                var distance = Vector2.Distance(screenPosition, PointerPosition);

                if (distance < Screen.dpi * GameConfigWrapper.GameConfig.minDistanceToPointer)
                {
                    goto return_result;
                }

                direction = new Vector2(PointerPosition.x - screenPosition.x, PointerPosition.y - screenPosition.y)
                    .normalized;
            }
            // Gamepad || Touchscreen
            else
            {
                direction = Direction;
            }

            return_result:

            var input = new Quantum.Input
            {
                Direction = new FPVector2(FP.FromFloat_UNSAFE(direction.x), FP.FromFloat_UNSAFE(direction.y)),
                ButtonBoost = ButtonBoost,
                ButtonShoot = ButtonShoot
            };

            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }

        public void OnCurrentControlSchemeChanged()
        {
        }
        
        public void OnPointer(InputAction.CallbackContext context)
        {
            PointerPosition = context.ReadValue<Vector2>();
        }

        public void OnDirection(InputAction.CallbackContext context)
        {
            Direction = context.ReadValue<Vector2>();
        }

        public void OnButtonBoost(InputAction.CallbackContext context)
        {
            ButtonBoost = context.ReadValue<float>() > 0;
        }

        public void OnButtonShoot(InputAction.CallbackContext context)
        {
            ButtonShoot = context.ReadValue<float>() > 0;
        }

        public enum EMode
        {
            KeyboardAndMouse,
            Touchscreen
        }

        public class OnPlayerInputModeChanged : EventBusEventBase
        {
            public readonly EMode Mode;

            private OnPlayerInputModeChanged(EMode mode)
            {
                Mode = mode;
            }

            public static void Raise(EMode mode)
            {
                EventBus.Raise(new OnPlayerInputModeChanged(mode));
            }
        }
    }

}
