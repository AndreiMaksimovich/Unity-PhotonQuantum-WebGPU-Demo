using System;
using System.Collections.Generic;
using System.Linq;

namespace Amax.QuantumDemo
{
    using Quantum;
    using UnityEngine;
    using Text = TMPro.TextMeshProUGUI;
    
    public class LevelView : QuantumSceneViewComponent, IEventBusListener<OnPlayerCharacterAdded>
    {
        [SerializeField] private Text text;
        
        private Dictionary<PlayerRef, EntityRef> _playerCharacters = new ();

        private void Awake()
        {
            EventBus.AddListener(this);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(this);
        }

        public override void OnInitialize()
        {
            QuantumEvent.Subscribe(this, (EventOnPlayerCharacterAdded data) => OnPlayerCharacterAdded(data));
            QuantumEvent.Subscribe(this, (EventOnPlayerCharacterRemoved data) => OnPlayerCharacterRemoved(data));
        }
        
        private void OnPlayerCharacterAdded(EventOnPlayerCharacterAdded data)
        {
            _playerCharacters.TryAdd(data.Player, data.PlayerCharacter);
        }

        private void OnPlayerCharacterRemoved(EventOnPlayerCharacterRemoved data)
        {
            _playerCharacters.Remove(data.Player);
        }
        
        public override void OnUpdateView()
        {
            var lines = new List<Tuple<int, string>>();
            
            foreach (var playerToCharacter in _playerCharacters)
            {
                if (!PredictedFrame.TryGet<PlayerCharacter>(playerToCharacter.Value, out var player)) continue;
                var color = PlayerColors.Instance.GetPlayerColor(playerToCharacter.Key);
                lines.Add(
                    new Tuple<int, string>(
                        player.Score, 
                        $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>Player {playerToCharacter.Key._index}: {player.Score}</color>"));
            }
            
            lines.Sort((value1, value2) => value1.Item1 < value2.Item1 ? 1 : -1);
            text.text = string.Join("\n", lines.Select(line => line.Item2));
        }

        public void OnEvent(OnPlayerCharacterAdded data)
        {
            _playerCharacters.TryAdd(data.Player, data.PlayerCharacter);
        }
    }
}