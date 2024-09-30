using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class PlayerSignalHandler : SystemSignalsOnly, 
        ISignalOnPlayerAdded, 
        ISignalOnPlayerRemoved,
        ISignalRespawnPlayerCharacter
    {
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            var data = f.GetPlayerData(player);
            var playerPrototype = f.GameConfig.playerPrototype;
            var playerEntity = f.Create(playerPrototype);

            f.Set(playerEntity, new PlayerCharacter() {PlayerRef = player});

            RespawnPlayerEntity(f, playerEntity);

            f.Events.OnPlayerCharacterAdded(player, playerEntity);
        }

        public void RespawnPlayerCharacter(Frame f, EntityRef playerEntityRef)
        {
            RespawnPlayerEntity(f, playerEntityRef);
        }

        private void RespawnPlayerEntity(Frame f, EntityRef playerEntity)
        {
            var transform = f.Unsafe.GetPointer<Transform2D>(playerEntity);
            transform->Position = MapUtils.GetValidRandomPlayerSpawnPosition(f);
            transform->Teleport(f, transform);
            f.Unsafe.GetPointer<PhysicsCollider2D>(playerEntity)->Enabled = true;
            f.Unsafe.GetPointer<PlayerCharacter>(playerEntity)->Score = 0;
        }
        
        public void OnPlayerRemoved(Frame f, PlayerRef player)
        {
            var filter = f.Filter<PlayerCharacter>();
            while (filter.Next(out var playerCharacterEntity, out var playerCharacter))
            {
                if (playerCharacter.PlayerRef != player) continue;
                f.Destroy(playerCharacterEntity);
                f.Events.OnPlayerCharacterRemoved(player, playerCharacterEntity);
            }
        }
    }
}