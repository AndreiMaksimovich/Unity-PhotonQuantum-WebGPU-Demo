using Photon.Deterministic;
using Quantum;

namespace Amax.QuantumDemo
{
    public class StaticDotView : QuantumEntityViewComponent
    {
        public FPVector2 Position => PredictedFrame.Get<Transform2D>(EntityRef).Position;
    }
}
