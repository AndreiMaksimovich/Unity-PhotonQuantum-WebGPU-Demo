using UnityEngine;

namespace Amax.QuantumDemo
{

    public class GameSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            AwakeInitialize();
        }

        protected virtual void AwakeInitialize()
        {
        }
    }

}
