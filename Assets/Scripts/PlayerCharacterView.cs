using Photon.Deterministic;

namespace Amax.QuantumDemo
{
    using UnityEngine;
    using Quantum;
    
    public unsafe class PlayerCharacterView : QuantumEntityViewComponent
    {
        public override void OnActivate(Frame frame)
        {
            var player = frame.Get<PlayerCharacter>(EntityRef);
            if (frame.IsPlayerVerifiedOrLocal(player.PlayerRef))
            {
                PlayerInputController.Instance.PlayerGameObject = gameObject;
                PlayerCamera.Instance.PlayerGameObject = gameObject;
            }
            OnPlayerCharacterAdded.Raise(player.PlayerRef, EntityRef);
        }

        private Quaternion _rotation = Quaternion.identity;
        
        public override void OnUpdateView()
        {
            var player = PredictedFrame.Get<PlayerCharacter>(EntityRef);
            var input = PredictedFrame.GetPlayerInput(player.PlayerRef);
            
            if (input->Direction != FPVector2.Zero)
            {
                var direction = input->Direction.ToUnityVector2();
                _rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
            }
            
            transform.rotation = _rotation;
            transform.localScale = Vector3.one * PlayerUtils.GetPlayerModelScale(player.Score);
        }
    }
}