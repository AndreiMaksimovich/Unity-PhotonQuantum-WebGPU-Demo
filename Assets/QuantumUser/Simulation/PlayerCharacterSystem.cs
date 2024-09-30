using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerCharacterSystem : SystemMainThreadFilter<PlayerCharacterSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform2D* Transform;
            public PlayerCharacter* Player;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var config = f.GameConfig;
            var input = f.GetPlayerInput(filter.Player->PlayerRef);
            
            if (input->Direction == FPVector2.Zero) return;
            
            UpdatePlayerMovement(f, ref filter, input, config);
            UpdatePlayerShooting(f, ref filter, input, config);
        }

        private void UpdatePlayerShooting(Frame f, ref Filter filter, Input* input, GameConfig config)
        {
            if (!input->ButtonShoot.IsDown || filter.Player->ShootFrame > f.Number) return;
            filter.Player->ShootFrame = f.Number + config.playerShootPeriodicity;
            f.Signals.SpawnPlayerCharacterProjectile(filter.Entity, input->Direction, filter.Transform->Position);
        }

        private void UpdatePlayerMovement(Frame f, ref Filter filter, Input* input, GameConfig config)
        {
            if (input->ButtonBoost.IsDown)
            {
                filter.Transform->Position += input->Direction * config.playerSpeed * f.DeltaTime * config.playerSpeedBoost;
            }
            else
            {
                filter.Transform->Position += input->Direction * config.playerSpeed * f.DeltaTime;
            }
        }
    }
}