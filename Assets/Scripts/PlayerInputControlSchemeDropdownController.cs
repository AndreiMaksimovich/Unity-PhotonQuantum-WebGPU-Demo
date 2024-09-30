using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Amax.QuantumDemo
{

    public class PlayerInputControlSchemeDropdownController : MonoBehaviour, IEventBusListener<PlayerInputController.OnPlayerInputModeChanged>
    {
        [SerializeField] private TMP_Dropdown dropdown;
        
        void Start()
        {
            EventBus.AddListener(this);

            var dropdownOptions = 
                (from PlayerInputController.EMode mode in Enum.GetValues(typeof(PlayerInputController.EMode)) select new TMP_Dropdown.OptionData(mode.ToString())).ToList();
            dropdown.AddOptions(dropdownOptions);
            RefreshView();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int value)
        {
            if ((int) PlayerInputController.Instance.Mode == value) return;
            PlayerInputController.Instance.Mode = (PlayerInputController.EMode) value;
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(this);
        }

        private void RefreshView()
        {
            if ((int) PlayerInputController.Instance.Mode == dropdown.value) return;
            dropdown.value = (int) PlayerInputController.Instance.Mode;
        }

        public void OnEvent(PlayerInputController.OnPlayerInputModeChanged data)
        {
            RefreshView();
        }
    }

}
