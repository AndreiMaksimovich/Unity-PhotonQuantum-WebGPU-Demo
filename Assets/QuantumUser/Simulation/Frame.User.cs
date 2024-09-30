using System;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class Frame
    {
#if UNITY_ENGINE
#endif
        public DotState[] dots;

        partial void InitUser()
        {
            if (dots != null) return;
            var config = FindAsset<GameConfig>(RuntimeConfig.GameConfig);
            dots = new DotState[config.dotCount];
        }
        
        partial void SerializeUser(FrameSerializer serializer)
        {
            serializer.Stream.SerializeArrayLength<DotState>(ref dots);
            for (var i = 0; i < dots.Length; i++)
            {
                
                DotState.Serialize(dots[i], serializer);
            }
        }

        partial void CopyFromUser(Frame frame)
        {
            Array.Copy(frame.dots, dots, frame.dots.Length);
        }
        
        //TODO optimise
        public GameConfig GameConfig => FindAsset<GameConfig>(RuntimeConfig.GameConfig);
    }
}