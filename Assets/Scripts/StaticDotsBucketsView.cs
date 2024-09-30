using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Unity.Collections;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class StaticDotsBucketsView : QuantumEntityViewComponent
    {
        private static readonly int InstanceColorBuffer = Shader.PropertyToID("_InstanceColorBuffer");
        private static readonly int InstanceLifeTimeBuffer = Shader.PropertyToID("_InstanceLifeTimeBuffer");
        private static readonly int InstanceIDOffset = Shader.PropertyToID("_InstanceIDOffset");
        
        private const int ElementsPerBatch = 200;

        [SerializeField] private bool isEnabled = true;
        
        [Header("Visualization")] [SerializeField]
        private Material dotMaterial;

        [SerializeField] private Mesh dotMesh;
        [SerializeField] private Vector2Int bucketGridSize = new Vector2Int(10, 10);

        private readonly Color[] _colors = {Color.blue, Color.green, Color.red, Color.cyan, Color.yellow,};
        private Bucket[,] _buckets;
        private FPVector2 _bucketSize;
        private FPVector2 _mapLeftBottomCorner;
        private GraphicData _data = new GraphicData();
        
        private float _simulationDeltaTime;
        private float _spawnDispwanAnimationDuration;

        private MaterialPropertyBlock _matProps;
        private RenderParams _renderParams;
        
        public override void OnActivate(Frame frame)
        {
            _simulationDeltaTime = (float) QuantumRunner.Default.Session.DeltaTimeDouble;
            _spawnDispwanAnimationDuration = GameConfigWrapper.GameConfig.spawnDispawnAnimationDuration;

            StartCoroutine(InitializationCoroutine());
        }

        private (int, int) GetBucketPosition(StaticDotView dotView)
        {
            var position = dotView.Position;
            return (
                FPMath.FloorToInt((- _mapLeftBottomCorner.X + position.X) / _bucketSize.X),
                FPMath.FloorToInt((- _mapLeftBottomCorner.Y +  position.Y) / _bucketSize.Y));
        }
            
        private IEnumerator InitializationCoroutine()
        {
            yield return null; // wait frame

            var mapSize = GameConfigWrapper.GameConfig.mapSize;
            _mapLeftBottomCorner = - mapSize / FP._2;
            _buckets = new Bucket[bucketGridSize.x, bucketGridSize.y];
            var views = new List<StaticDotView>[bucketGridSize.x, bucketGridSize.y];
            _bucketSize = new FPVector2(mapSize.X / bucketGridSize.x, mapSize.Y / bucketGridSize.y);
            
            for (var x = 0; x < bucketGridSize.x; x++)
            {
                for (var y = 0; y < bucketGridSize.y; y++)
                {
                    _buckets[x, y] = new Bucket()
                    {
                        Position = new Vector2Int(x, y),
                        Rect = new Rect(
                            _mapLeftBottomCorner.X.AsFloat + x * _bucketSize.X.AsFloat, 
                            _mapLeftBottomCorner.X.AsFloat + y * _bucketSize.X.AsFloat, 
                            _bucketSize.X.AsFloat, 
                            _bucketSize.X.AsFloat)
                    };
                    views[x, y] = new List<StaticDotView>();
                }
            }
            
            var entityCount = 0;
            
            foreach (var staticDotView in FindObjectsByType<StaticDotView>(FindObjectsSortMode.None))
            {
                var (gridX, gridY) = GetBucketPosition(staticDotView);
                views[gridX, gridY].Add(staticDotView);
                entityCount++;
            }

            _data.Initialize(entityCount);
            var globalIndex = 0;
            
            for (var x = 0; x < bucketGridSize.x; x++)
            {
                for (var y = 0; y < bucketGridSize.y; y++)
                {
                    var bucketViews = views[x, y];
                    var bucket = _buckets[x, y];
                    bucket.Offset = globalIndex;
                    var index = 0;
                    foreach (var dotView in bucketViews)
                    {
                        bucket.Entities.Add(dotView.EntityRef);
                        _data.Entities.Add(dotView.EntityRef);
                        _data.Matrices.Add(Matrix4x4.TRS(dotView.transform.position, Quaternion.identity, dotView.transform.lossyScale));
                        _data.ColorNativeArray[globalIndex] = _colors[globalIndex % _colors.Length];
                        Destroy(dotView);
                        index++;
                        globalIndex++;
                    }
                    bucket.EntityCount = bucket.Entities.Count;
                }
            }
            
            _renderParams = new RenderParams(dotMaterial) { matProps = _data.MaterialProps };
            
            // Color data is "static"
            _data.ColorBuffer.SetData(_data.ColorNativeArray);
            dotMaterial.SetBuffer(InstanceColorBuffer, _data.ColorBuffer);
        }

        private void OnDestroy()
        {
            _data.Dispose();
        }

        private bool CheckRectVisibility(Rect rect)
        {
            // Note: this method is "oversimplified" and is used for testing purposes only
            foreach (var point in new Vector2[]
                     {
                         new(rect.center.x, rect.center.y),
                         new (rect.x, rect.y),
                         new (rect.x + rect.width, rect.y),
                         new (rect.x, rect.y + rect.height),
                         new (rect.x + rect.width, rect.y + rect.height),
                     })
            {
                var viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(point.x, 0, point.y));
                if (viewportPoint.x is > -0.1f and < 1.1f && viewportPoint.y is > -0.1f and < 1.1f) return true;
            }
            return false;
        }
        
        public override void OnUpdateView()
        {
            if (!isEnabled) return;
            if (_buckets == null) return;

            var currentFrameNumber = PredictedFrame.Number;
            
            foreach (var bucket in _buckets)
            {
                // Skip invisible bucket
                if (!CheckRectVisibility(bucket.Rect))
                {
                    continue;
                }
                
                for (var i = 0; i < bucket.EntityCount; i++)
                {
                    var dotState = PredictedFrame.Get<StaticDot>(bucket.Entities[i]);
                    var index = bucket.Offset + i;
                    _data.LifeTimeNativeArray[index] = (currentFrameNumber - dotState.Frame) * _simulationDeltaTime;
                    if (Mathf.Abs(_data.LifeTimeNativeArray[index]) > _spawnDispwanAnimationDuration)
                    {
                        _data.MatrixNativeArray[index] = dotState.IsAlive ? _data.Matrices[index] : Matrix4x4.zero;
                    }
                    else
                    {
                        var multiplier = (dotState.IsAlive ? _data.LifeTimeNativeArray[index] : _spawnDispwanAnimationDuration - _data.LifeTimeNativeArray[index]) / _spawnDispwanAnimationDuration;
                        _data.MatrixNativeArray[index] = _data.Matrices[index] * Matrix4x4.Scale(Vector3.one * multiplier);
                    }
                }
                
                _data.LifeTimeBuffer.SetData(_data.LifeTimeNativeArray);
                dotMaterial.SetBuffer(InstanceLifeTimeBuffer, _data.LifeTimeBuffer);
                
                var batchCount = (int) MathF.Ceiling((float) bucket.EntityCount / ElementsPerBatch);

                for (var batch = 0; batch < batchCount; batch++)
                {
                    
                    var indexFrom = batch * ElementsPerBatch;
                    var indexTo = Math.Min(bucket.EntityCount, indexFrom + ElementsPerBatch);
                    var elementsInBatch = indexTo - indexFrom;
                    
                    _data.MaterialProps.SetInteger(InstanceIDOffset, bucket.Offset + indexFrom);

                    Graphics.RenderMeshInstanced
                        (_renderParams, dotMesh, 0, _data.MatrixNativeArray, elementsInBatch, bucket.Offset + indexFrom);
                }
            }
        }
        
        private class GraphicData
        {
            public MaterialPropertyBlock MaterialProps;
            
            public readonly List<EntityRef> Entities = new ();
            
            public NativeArray<Color> ColorNativeArray;
            public GraphicsBuffer ColorBuffer;
            
            public NativeArray<float> LifeTimeNativeArray;
            public GraphicsBuffer LifeTimeBuffer;
            
            public NativeArray<Matrix4x4> MatrixNativeArray;
            public readonly List<Matrix4x4> Matrices = new ();

            public void Dispose()
            {
                ColorBuffer.Dispose();
                LifeTimeNativeArray.Dispose();
                MatrixNativeArray.Dispose();
            }
            
            public void Initialize(int count)
            {
                MaterialProps = new MaterialPropertyBlock();
                
                ColorNativeArray = new NativeArray<Color>(count, Allocator.Persistent);
                ColorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float) * 4);

                LifeTimeNativeArray = new NativeArray<float>(count, Allocator.Persistent);
                LifeTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, sizeof(float));
            
                MatrixNativeArray = new NativeArray<Matrix4x4>(count, Allocator.Persistent);
            }
        }
        
        private class Bucket
        {
            public Rect Rect;
            public Vector2Int Position;
            public int Offset;
            public List<EntityRef> Entities = new();
            public int EntityCount;
        }
    }
}