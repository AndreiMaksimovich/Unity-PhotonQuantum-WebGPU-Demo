using Quantum;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class GameConfigWrapper: GameSingleton<GameConfigWrapper>
    {
        [SerializeField] private GameConfig _gameConfig;
        public static GameConfig GameConfig => Instance._gameConfig;
    }
}