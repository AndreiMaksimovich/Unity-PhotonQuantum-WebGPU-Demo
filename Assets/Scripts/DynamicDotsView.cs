using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using Unity.Collections;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class DynamicDotsView: QuantumEntityViewComponent
    {
        private static readonly int InstanceColorBuffer = Shader.PropertyToID("_InstanceColorBuffer");
        private static readonly int InstanceLifeTimeBuffer = Shader.PropertyToID("_InstanceLifeTimeBuffer");
        private static readonly int InstanceIDOffset = Shader.PropertyToID("_InstanceIDOffset");

        private const int ElementsPerBatch = 200;

        [Header("Visualization")]
        [SerializeField] private Material dotMaterial;
        [SerializeField] private Mesh dotMesh;
        
        private static readonly Color[] Colors = new Color[] { Color.blue, Color.green, Color.red, Color.cyan, Color.yellow,  };
        private readonly Dictionary<int, GameObject> _dots = new Dictionary<int, GameObject>();
        
        private NativeArray<Color> _colorNativeArray;
        private GraphicsBuffer _colorBuffer;
        
        private NativeArray<float> _lifeTimeNativeArray;
        private GraphicsBuffer _lifeTimeBuffer;
        
        private NativeArray<Matrix4x4> _matricesNativeArray;
        private readonly List<Matrix4x4> _matrices = new List<Matrix4x4>();
        
        private float _simulationDeltaTime;
        private float _spawnDispwanAnimationDuration;
        
        private MaterialPropertyBlock _matProps;
        private RenderParams _renderParams;

        private int _elementCount;
        
        public override void OnActivate(Frame frame)
        {
            base.OnActivate(frame);
            StartCoroutine(InitializationCoroutine());
        }
        
        private IEnumerator InitializationCoroutine()
        {
            yield return null;
            _simulationDeltaTime = (float) QuantumRunner.Default.Session.DeltaTimeDouble;
            _spawnDispwanAnimationDuration = GameConfigWrapper.GameConfig.spawnDispawnAnimationDuration;
            Initialize();
        }

        private void Initialize()
        {
            foreach (var dotView in FindObjectsByType<DynamicDotView>(FindObjectsSortMode.None))
            {
                var index = VerifiedFrame.Get<DynamicDot>(dotView.EntityRef).Index;
                _dots.TryAdd(index, dotView.gameObject);
            }
            
            foreach (var dotGameObject in _dots.Values)
            {
                Destroy(dotGameObject);
            }
            
            _elementCount = _dots.Count;
            if (_elementCount == 0) return;
            
            _colorNativeArray = new NativeArray<Color>(_elementCount, Unity.Collections.Allocator.Persistent);
            _colorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _elementCount, sizeof(float) * 4);

            _lifeTimeNativeArray = new NativeArray<float>(_elementCount, Unity.Collections.Allocator.Persistent);
            _lifeTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _elementCount, sizeof(float));
            
            _matricesNativeArray = new NativeArray<Matrix4x4>(_elementCount, Unity.Collections.Allocator.Persistent);

            for (var index = 0; index < _elementCount; index++)
            {
                var dotGameObject = _dots[index];
                var modelTransform = dotGameObject.transform;
                var matrix = Matrix4x4.TRS(modelTransform.position, Quaternion.identity, modelTransform.localScale);
                _matricesNativeArray[index] = matrix;
                _matrices.Add(matrix);
                _colorNativeArray[index] = Colors[index % Colors.Length];
            }
            
            _matProps = new MaterialPropertyBlock();
            _renderParams = new RenderParams(dotMaterial) { matProps = _matProps };
        }

        private void OnDestroy()
        {
            _colorNativeArray.Dispose();
            _lifeTimeNativeArray.Dispose();
            _matricesNativeArray.Dispose();
        }
        
        public override void OnUpdateView()
        {
            if (_dots.Count == 0) return;
            var currentFrameNumber = PredictedFrame.Number;
            
            for (var i = 0; i < _elementCount; i++)
            {
                var dotState = PredictedFrame.dots[i];
                _lifeTimeNativeArray[i] = (currentFrameNumber - dotState.Frame) * _simulationDeltaTime;
                if (Mathf.Abs(_lifeTimeNativeArray[i]) > _spawnDispwanAnimationDuration)
                {
                    _matricesNativeArray[i] = dotState.IsAlive ? _matrices[i] : Matrix4x4.zero;
                }
                else
                {
                    var multiplier = (dotState.IsAlive ? _lifeTimeNativeArray[i] : _spawnDispwanAnimationDuration - _lifeTimeNativeArray[i]) / _spawnDispwanAnimationDuration;
                    _matricesNativeArray[i] = _matrices[i] * Matrix4x4.Scale(Vector3.one * multiplier);
                }
            }
                
            _colorBuffer.SetData(_colorNativeArray);
            dotMaterial.SetBuffer(InstanceColorBuffer, _colorBuffer);
            
            _lifeTimeBuffer.SetData(_lifeTimeNativeArray);
            dotMaterial.SetBuffer(InstanceLifeTimeBuffer, _lifeTimeBuffer);
            
            var batchCount =  (int) Math.Ceiling(_elementCount / (float) ElementsPerBatch);

            for (var batch = 0; batch < batchCount; batch++)
            {
                var indexFrom = batch * ElementsPerBatch;
                var indexTo = Math.Min(_dots.Count, indexFrom + ElementsPerBatch);
                var elementsInBatch = indexTo - indexFrom;
                
                _matProps.SetInteger(InstanceIDOffset, indexFrom);
                
                Graphics.RenderMeshInstanced
                    (_renderParams, dotMesh, 0, _matricesNativeArray, elementsInBatch, indexFrom);
            }
        }
    }
}