// Copyright (c) 2022 Andrei Maksimovich
// See LICENSE.md

using System.Runtime.InteropServices;

namespace Quantum
{
    
    [StructLayout(LayoutKind.Explicit)]
    public struct DotState
    {
        
        public const int SIZE = 5;
        
        [FieldOffset(0)]
        public int Frame;

        [FieldOffset(4)] 
        public byte IsAliveValue;

        public bool IsAlive
        {
            get => IsAliveValue != 0;
            set => IsAliveValue = value ? (byte) 1 : (byte) 0;
        }
        
        public static unsafe void Serialize(void* ptr, FrameSerializer serializer)
        {
            var dotState = (DotState*)ptr;
            serializer.Stream.Serialize(&dotState->Frame);
            serializer.Stream.Serialize(&dotState->IsAliveValue);
        }
        
        public static unsafe void Serialize(DotState dotState, FrameSerializer serializer)
        {
            serializer.Stream.Serialize(&dotState.Frame);
            serializer.Stream.Serialize(&dotState.IsAliveValue);
        }
    }
}