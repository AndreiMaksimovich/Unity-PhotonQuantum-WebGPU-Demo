using Quantum;

namespace Amax.QuantumDemo
{
    using UnityEngine;
    
    public class DynamicDotView : QuantumEntityViewComponent
    {
        private void Awake()
        {
            SetStatic(gameObject);
        }

        private void SetStatic(GameObject go)
        {
            gameObject.isStatic = true;
            foreach (Transform child in go.transform)
            {
                SetStatic(child.gameObject);
            }
        }
    }
}
