using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerProjectileSignalHandler : SystemSignalsOnly, ISignalSpawnPlayerCharacterProjectile, ISignalDestroyPlayerCharacterProjectile
    {
        
        public void SpawnPlayerCharacterProjectile(Frame f, EntityRef playerCharacterEntityRef, FPVector2 direction, FPVector2 position)
        {
            var config = f.GameConfig;
            var projectile = f.Create(config.playerProjectilePrototype);

            f.Set(projectile, new PlayerCharacterProjectile()
            {
                PlayerCharacter = playerCharacterEntityRef,
                Direction = direction,
                DeathFrame = f.Number + config.playerProjectileLifeTime
            });

            var transform = f.Get<Transform2D>(projectile);
            transform.Position = position;
            f.Set(projectile, transform);
        }

        public void DestroyPlayerCharacterProjectile(Frame f, EntityRef projectileEntityRef)
        {
            f.Destroy(projectileEntityRef);
        }
    }
}
