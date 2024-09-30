// Copyright (c) 2022 Andrei Maksimovich
// See LICENSE.md

using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public static class PlayerUtils
    {
        private static FP DefaultPlayerColliderSize = FP._0_75;
        private static FP GetPlayerColliderSizePower = FP._0_50;
        
        public static float GetPlayerModelScale(int score)
        {
            return (GetPlayerColliderSize(score)/DefaultPlayerColliderSize).AsFloat;
        }

        public static FP GetPlayerColliderSize(int score)
        {
            if (score == 0) return DefaultPlayerColliderSize;
            return DefaultPlayerColliderSize * FPMath.Exp(GetPlayerColliderSizePower * FPMath.Ln(score) / FP._3);
        }
    }
}