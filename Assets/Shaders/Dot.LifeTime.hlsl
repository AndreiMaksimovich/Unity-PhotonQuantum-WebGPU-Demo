#if UNITY_ANY_INSTANCING_ENABLED

StructuredBuffer<float> _InstanceLifeTimeBuffer;

void GetInstanceLifeTime_float(out float lifeTime)
{
    lifeTime = _InstanceLifeTimeBuffer[unity_InstanceID + _InstanceIDOffset];
}

#else

void GetInstanceLifeTime_float(out float lifeTime)
{
    lifeTime = 1;
}

#endif