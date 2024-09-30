namespace Quantum
{
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class StaticDotSystem : SystemMainThreadFilter<StaticDotSystem.Filter>
    {
        
        public struct Filter
        {
            public EntityRef Entity;
            public StaticDot* StaticDot;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            var config = f.GameConfig;
            if (filter.StaticDot->IsAlive || f.Number - filter.StaticDot->Frame <= config.dotRespawnDuration) return;
            
            filter.StaticDot->IsAlive = true;
            filter.StaticDot->Frame = f.Number;
        }
    }
}
