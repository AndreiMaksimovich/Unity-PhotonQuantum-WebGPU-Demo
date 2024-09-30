// Copyright (c) 2022 Andrei Maksimovich
// See LICENSE.md

using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public static unsafe class MapUtils
    {

        public static unsafe FPVector2 GetValidRandomPlayerSpawnPosition(Frame f)
        {
            var config = f.GameConfig;
            var region = config.mapSize / FP._4;
            return new FPVector2(f.RNG->Next(-region.X, region.X), f.RNG->Next(-region.Y, region.Y));
        }
        
        
    }
}