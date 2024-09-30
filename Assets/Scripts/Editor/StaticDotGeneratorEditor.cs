using System.Linq;
using Quantum;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Amax.QuantumDemo.Editor
{

    using UnityEditor;
    
    [CustomEditor(typeof(StaticDotGenerator))]
    public class StaticDotGeneratorEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (Application.isPlaying) return;

            if (GUILayout.Button("Generate"))
            {
                GenerateDots();
            }
            
            if (GUILayout.Button("Clear"))
            {
                Clear(((StaticDotGenerator) target).transform);
            }
        }

        private void GenerateDots()
        {
            var generator = (StaticDotGenerator) target;
            
            // Clear
            if (generator.clearAutomatically) Clear(generator.transform);
            
            var mapSize = generator.gameConfig.mapSize.ToUnityVector2();

            for (var i = 0; i < generator.count; i++)
            {
                var position = new Vector3(
                    mapSize.x * (-0.5f + Random.value),
                    0,
                    mapSize.y * (-0.5f + Random.value));
                
                var dotGameObject = Instantiate(generator.prefab, position, Quaternion.identity, generator.transform);
                var entityPrototype = dotGameObject.GetComponent<QuantumEntityPrototype>();
                var staticDot = entityPrototype.GetComponent<QPrototypeStaticDot>();
                staticDot.Prototype.IsAlive = true;
                staticDot.Prototype.Index = i;
            }
        }

        private void Clear(Transform root)
        {
            var elements = (from Transform child in root select child.gameObject).ToList();
            foreach (var gameObject in elements)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}