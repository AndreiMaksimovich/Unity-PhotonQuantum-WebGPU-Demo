using UnityEngine;

namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class DyanmicDotSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            var config = f.GameConfig;
            if (config.dotCount == 0) return;
            
            var startIndex = f.Number * config.dotsToUpdatePerFrame % config.dotCount;
            var endIndex = Mathf.Min(startIndex + config.dotsToUpdatePerFrame, config.dotCount);

            for (var i = startIndex; i < endIndex; i++)
            {
                if (!f.dots[i].IsAlive && f.Number - f.dots[i].Frame > config.dotRespawnDuration)
                {
                    f.dots[i] = new DotState()
                    {
                        IsAlive = true,
                        Frame = f.Number
                    };
                }
            }
        }
    }
}
