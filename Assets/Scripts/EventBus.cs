using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Amax.QuantumDemo {

    // ----------------------------------------------------------------------------------------
    // Event Base

    public class EventBusEventBase {}
    
    public abstract class EventBusEventBaseWithBody<T>
    {
        public T Body { get; protected set; }
    }
    
    public delegate void EventBusEventCallback<E>(E data) where E:EventBusEventBase;

    public interface IEventBusListenerBase {}
    
    public interface IEventBusListener<E> : IEventBusListenerBase
    {
        void OnEvent(E data);
    }

    // ----------------------------------------------------------------------------------------

    // ----- Unity Events

    [Serializable]
    public class UnityEventBoolean : UnityEvent<bool> { }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [Serializable]
    public class UnityEventMonoBehaviour : UnityEvent<MonoBehaviour> { }

    // ----------------------------------------------------------------------------------------

    public static class EventBus {

        private static readonly Dictionary<Type, List<IEventBusListenerBase>> listeners = new Dictionary<Type, List<IEventBusListenerBase>>();
        private static readonly Dictionary<Type, List<Delegate>> callbacks = new Dictionary<Type, List<Delegate>>();

        // ------------------------------------------

        public static void AddListener<Type>(IEventBusListener<Type> listener) where Type:EventBusEventBase{
            var type = typeof(Type);
            if (!listeners.ContainsKey(type)) listeners.Add(type, new List<IEventBusListenerBase>());
            var list = listeners[type];
            if (!list.Contains(listener)) list.Add(listener);
        }

        public static void RemoveListener<Type>(IEventBusListener<Type> listener) where Type : EventBusEventBase {
            var type = typeof(Type);
            if (!listeners.ContainsKey(type)) return;
            listeners[type].Remove(listener);
        }

        // ------------------------------------------

        public static void AddCallback<Type>(EventBusEventCallback<Type> callback) where Type : EventBusEventBase {
            var type = typeof(Type);
            if (!callbacks.ContainsKey(type)) callbacks.Add(type, new List<Delegate>());
            var list = callbacks[type];
            if (!list.Contains(callback)) list.Add(callback);
        }

        public static void RemoveCallback<Type>(EventBusEventCallback<Type> callback) where Type : EventBusEventBase {
            var type = typeof(Type);
            if (!callbacks.ContainsKey(type)) return;
            callbacks[type].Remove(callback);
        }

        // ------------------------------------------

        public static void Clear<Type>() where Type:EventBusEventBase {
            listeners.Remove(typeof(Type));
            callbacks.Remove(typeof(Type));
        }   

        public static void Clear() {
            listeners.Clear();
            callbacks.Clear();
        }

        // ------------------------------------------

        public static void Raise<Type>(Type data) where Type:EventBusEventBase {
            var type = data.GetType();
            if (listeners.ContainsKey(type)) {
                foreach (var listener in listeners[type]) {
                    (listener as IEventBusListener<Type>).OnEvent(data);
                }
            }
            if (callbacks.ContainsKey(type)) {
                foreach (var callback in callbacks[type]) {
                    (callback as EventBusEventCallback<Type>).Invoke(data);
                }
            }
        }

        // ------------------------------------------
        
    }


}