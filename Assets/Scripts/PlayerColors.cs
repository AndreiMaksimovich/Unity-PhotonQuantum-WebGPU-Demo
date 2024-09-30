using Quantum;
using UnityEngine;

namespace Amax.QuantumDemo
{

    public class PlayerColors : GameSingleton<PlayerColors>
    {

        [SerializeField] private Color[] colors = new[]
        {
            Color.blue,
            Color.green,
            Color.red,
            Color.yellow,
            Color.white,
            Color.cyan,
        };

        public Color GetPlayerColor(PlayerRef player)
        {
            if (!player.IsValid) return Color.magenta;
            return colors[(int) player._index - 1];
        }

    }

}