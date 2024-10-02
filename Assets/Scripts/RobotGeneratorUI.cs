using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Amax.QuantumDemo
{
    public class RobotGeneratorUI : MonoBehaviour
    {
        [SerializeField] private Button buttonPlus;
        [SerializeField] private Button buttonMinus;
        [SerializeField] private TMP_Text countText;

        private void Start()
        {
            buttonMinus.onClick.AddListener(() =>
            {
                AnimatedRobotKyleGenerator.Instance.UpdateRobotCount(AnimatedRobotKyleGenerator.Instance.RobotRowCount - 1);
            });
            
            buttonPlus.onClick.AddListener(() =>
            {
                AnimatedRobotKyleGenerator.Instance.UpdateRobotCount(AnimatedRobotKyleGenerator.Instance.RobotRowCount + 1);
            });
        }

        private void Update()
        {
            countText.text = $"{AnimatedRobotKyleGenerator.Instance.RobotCount}";
        }
    }

}
