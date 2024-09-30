using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerProjectileSystem : SystemMainThreadFilter<PlayerProjectileSystem.Filter>
    {
        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.Projectile->DeathFrame < f.Number)
            {
                f.Destroy(filter.Entity);
                return;
            }

            filter.Transform->Position += filter.Projectile->Direction * f.GameConfig.playerProjectileSpeed * f.DeltaTime;
        }
        
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerCharacterProjectile* Projectile;
            public Transform2D* Transform;
        }
    }
}
