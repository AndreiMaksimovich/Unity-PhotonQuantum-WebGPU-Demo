using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerCharacterCollisionsSystem : SystemSignalsOnly, ISignalOnTriggerEnter2D
    {
        public void OnTriggerEnter2D(Frame f, TriggerInfo2D info)
        {
            if (!f.TryGet<PlayerCharacter>(info.Entity, out var playerCharacter)) return;

            // Projectile
            if (f.TryGet<PlayerCharacterProjectile>(info.Other, out var projectile))
            {
                if (info.Entity == projectile.PlayerCharacter) return;
                f.Signals.DestroyPlayerCharacterProjectile(info.Other);
                f.Signals.RespawnPlayerCharacter(info.Entity);
                return;
            }
            
            // Dynamic dot
            if (f.TryGet<DynamicDot>(info.Other, out var dot))
            {
                var index = dot.Index;
                var dotState = f.dots[index];
                if (dotState.IsAlive)
                {
                    // Despawn dot
                    f.dots[index] = new DotState()
                    {
                        IsAlive = false,
                        Frame = f.Number
                    };
                    
                    // Set Score
                    playerCharacter.Score++;
                    f.Set(info.Entity, playerCharacter);
                    
                    var collider2D = f.Get<PhysicsCollider2D>(info.Entity);
                    collider2D.Shape.Circle.Radius = PlayerUtils.GetPlayerColliderSize(playerCharacter.Score);
                    //Debug.Log($"Score={player.Score}, Radius={collider2D.Shape.Circle.Radius }");
                    f.Set(info.Entity, collider2D);
                }
                return;
            }

            // Static dot
            if (f.TryGet<StaticDot>(info.Other, out var staticDot) && staticDot.IsAlive)
            {
                // Update static dot
                staticDot.IsAlive = false;
                staticDot.Frame = f.Number;
                f.Set(info.Other, staticDot);
                
                // Set Score
                playerCharacter.Score++;
                f.Set(info.Entity, playerCharacter);
                
                var collider2D = f.Get<PhysicsCollider2D>(info.Entity);
                collider2D.Shape.Circle.Radius = PlayerUtils.GetPlayerColliderSize(playerCharacter.Score);
                f.Set(info.Entity, collider2D);
            }
        }
    }
}