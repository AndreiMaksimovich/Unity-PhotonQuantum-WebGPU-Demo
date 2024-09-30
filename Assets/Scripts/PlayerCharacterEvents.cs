using Quantum;

namespace Amax.QuantumDemo
{

    public abstract class PlayerCharacterBaseEvent: EventBusEventBase {
        public readonly PlayerRef Player;
        public readonly EntityRef PlayerCharacter;

        protected PlayerCharacterBaseEvent(PlayerRef player, EntityRef playerCharacter)
        {
            Player = player;
            PlayerCharacter = playerCharacter;
        }
    }
    

    public class OnPlayerCharacterAdded : PlayerCharacterBaseEvent
    {
        private OnPlayerCharacterAdded(PlayerRef player, EntityRef playerCharacter): base(player, playerCharacter) { }
        public static void Raise(PlayerRef player, EntityRef playerCharacter)
        {
            EventBus.Raise(new OnPlayerCharacterAdded(player, playerCharacter));
        }
    }

    public class OnPlayerCharacterRemoved : PlayerCharacterBaseEvent
    {
        private OnPlayerCharacterRemoved(PlayerRef player, EntityRef playerCharacter): base(player, playerCharacter) { }
        public static void Raise(PlayerRef player, EntityRef playerCharacter)
        {
            EventBus.Raise(new OnPlayerCharacterRemoved(player, playerCharacter));
        }
    }
    
}