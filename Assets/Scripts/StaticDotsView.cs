using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using Unity.Collections;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class StaticDotsView: QuantumEntityViewComponent
    {
        private static readonly int InstanceColorBuffer = Shader.PropertyToID("_InstanceColorBuffer");
        private static readonly int InstanceLifeTimeBuffer = Shader.PropertyToID("_InstanceLifeTimeBuffer");
        private static readonly int InstanceIDOffset = Shader.PropertyToID("_InstanceIDOffset");

        private const int ElementsPerBatch = 200;
        
        [SerializeField] private bool isEnabled = true;

        [Header("Visualization")]
        [SerializeField] private Material dotMaterial;
        [SerializeField] private Mesh dotMesh;
        
        private readonly Color[] _colors = { Color.blue, Color.green, Color.red, Color.cyan, Color.yellow,  };
        private readonly List<EntityRef> _staticDotEntities = new ();
        
        private NativeArray<Color> _colorNativeArray;
        private GraphicsBuffer _colorBuffer;
        
        private NativeArray<float> _lifeTimeNativeArray;
        private GraphicsBuffer _lifeTimeBuffer;
        
        private NativeArray<Matrix4x4> _matricesNativeArray;
        private List<Matrix4x4> _matrices = new ();
        
        private float _simulationDeltaTime;
        private float _spawnDispwanAnimationDuration;
        
        private MaterialPropertyBlock _matProps;
        private RenderParams _renderParams;

        private int _elementCount;
        
        public override void OnActivate(Frame frame)
        {
            _simulationDeltaTime = (float) QuantumRunner.Default.Session.DeltaTimeDouble;
            _spawnDispwanAnimationDuration = GameConfigWrapper.GameConfig.spawnDispawnAnimationDuration;
            StartCoroutine(InitializationCoroutine());
        }
        
        private IEnumerator InitializationCoroutine()
        {
            yield return null; // wait frame
            
            foreach (var staticDotView in FindObjectsByType<StaticDotView>(FindObjectsSortMode.None))
            {
                _staticDotEntities.Add(staticDotView.EntityRef);
                var modelTransform = staticDotView.transform;
                _matrices.Add(Matrix4x4.TRS(modelTransform.position, Quaternion.identity, modelTransform.localScale));
                Destroy(staticDotView.gameObject);
            }
            
            _elementCount = _staticDotEntities.Count;
            
            _colorNativeArray = new NativeArray<Color>(_elementCount, Unity.Collections.Allocator.Persistent);
            _colorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _elementCount, sizeof(float) * 4);

            _lifeTimeNativeArray = new NativeArray<float>(_elementCount, Unity.Collections.Allocator.Persistent);
            _lifeTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _elementCount, sizeof(float));
            
            _matricesNativeArray = new NativeArray<Matrix4x4>(_elementCount, Unity.Collections.Allocator.Persistent);

            for (var index = 0; index < _elementCount; index++)
            {
                _matricesNativeArray[index] = _matrices[index];
                _colorNativeArray[index] = _colors[index % _colors.Length];
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
            if (!isEnabled) return;
            if (_staticDotEntities.Count == 0) return;
            var currentFrameNumber = PredictedFrame.Number;
            
            for (var i = 0; i < _elementCount; i++)
            {
                var dotState = PredictedFrame.Get<StaticDot>(_staticDotEntities[i]);
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
            
            var batchCount =  (int) Math.Ceiling((float) _elementCount / ElementsPerBatch);

            for (var batch = 0; batch < batchCount; batch++)
            {
                var indexFrom = batch * ElementsPerBatch;
                var indexTo = Math.Min(_staticDotEntities.Count, indexFrom + ElementsPerBatch);
                var elementsInBatch = indexTo - indexFrom;
                
                _matProps.SetInteger(InstanceIDOffset, indexFrom);
                
                Graphics.RenderMeshInstanced
                    (_renderParams, dotMesh, 0, _matricesNativeArray, elementsInBatch, indexFrom);
            }
        }
    }
}