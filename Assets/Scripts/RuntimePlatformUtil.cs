using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Amax.QuantumDemo
{

    public class RuntimePlatformUtil
    {
        [DllImport("__Internal")]
        private static extern bool IsUserAgentMobile();

        public static bool IsMobile()
        {
            // Editor
#if UNITY_EDITOR
            return false;
#endif
            
            // Android & iOS
#if UNITY_ANDROID || UNITY_IPHONE
            return true;
#endif

            // WebGL
#if UNITY_WEBGL
            try
            {
                return IsUserAgentMobile();
            }
            catch (Exception e)
            {
                Debug.LogError("JSLIB IsUserAgentMobile Failed");
                Debug.LogException(e);
                return false;
            }
#endif
            // Default
            return false;
        }
        
    }

}
