namespace Amax.QuantumDemo
{
    using UnityEngine;
    using Quantum;
    
    public class StaticDotClassicRenderingView : QuantumSceneViewComponent
    {

        [SerializeField] private Transform modelRoot;
        [SerializeField] private MeshRenderer modelRenderer;
        private Vector3 _initialScale;
        private float _simulationDeltaTime;
        private float _spawnDispwanAnimationDuration;
        
        private static readonly Color[] Colors = { Color.blue, Color.green, Color.red, Color.cyan, Color.yellow,  };
        
        public override void OnActivate(Frame frame)
        {
            _simulationDeltaTime = (float) QuantumRunner.Default.Session.DeltaTimeDouble;
            _spawnDispwanAnimationDuration = GameConfigWrapper.GameConfig.spawnDispawnAnimationDuration;
            var staticDot = PredictedFrame.Get<StaticDot>(_entityView.EntityRef);
            _initialScale = modelRoot.localScale;
            modelRenderer.material.SetColor(Shader.PropertyToID("_DotColor"), Colors[staticDot.Index % Colors.Length]);
        }

        public override void OnUpdateView()
        {
            var staticDot = PredictedFrame.Get<StaticDot>(_entityView.EntityRef);
            var lifeTime = (PredictedFrame.Number - staticDot.Frame) * _simulationDeltaTime;
            
            if (Mathf.Abs(lifeTime) > _spawnDispwanAnimationDuration)
            {
                modelRoot.localScale = staticDot.IsAlive ? _initialScale : Vector3.zero;
            }
            else
            {
                var multiplier = (staticDot.IsAlive ? lifeTime : _spawnDispwanAnimationDuration - lifeTime) / _spawnDispwanAnimationDuration;
                modelRoot.localScale = _initialScale * multiplier;
            }
        }
    }
}
