// Copyright (c) 2022 Andrei Maksimovich
// See LICENSE.md

using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum
{
    [Preserve]
    public unsafe class DynamicDotInitializationSystem: SystemSignalsOnly
    {
        
        public override void OnInit(Frame f)
        {
            base.OnInit(f);
            
            var config = f.FindAsset<GameConfig>(f.RuntimeConfig.GameConfig);
            var dotCount = config.dotCount;
            
            f.dots = new DotState[dotCount];

            for (var i = 0; i < dotCount; i++)
            {
                f.dots[i] = new DotState()
                {
                    Frame = -100,
                    IsAlive = true
                };
                
                var dotEntity = f.Create(config.dotPrototype);
                
                var dot = f.Get<DynamicDot>(dotEntity);
                dot.Index = i;
                f.Set(dotEntity, dot);
                
                var transform = f.Get<Transform2D>(dotEntity);
                transform.Position = new FPVector2(f.RNG->Next() * config.mapSize.X, f.RNG->Next() * config.mapSize.Y) - config.mapSize / FP._2;
                f.Set(dotEntity, transform);
            }

            f.Events.OnDotsInstantiated();
        }
        
    }
}