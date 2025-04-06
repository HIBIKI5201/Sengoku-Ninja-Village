using System;
using System.Collections.Generic;
using System.Reflection;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage.Runtime.System
{
    
    
    public static class InputManager
    {
        private static Dictionary<InputKind, Delegate> _actionListDictionary = new Dictionary<InputKind, Delegate>();
        
        public static Action RegisterAction(InputKind input)
        {
            _actionListDictionary.TryAdd(input, null);
            return () => Invoke(input);
        }
        
        public static Action RegisterAction<T>(InputKind input, T value)
        {
            _actionListDictionary.TryAdd(input, null);
            return () => Invoke(input, value);
        }

        public static void AddAction(InputKind input, Action action)
        {
            _actionListDictionary.TryAdd(input, action);
            var contains = _actionListDictionary.TryGetValue(input, out Delegate value);

            MethodInfo info = _actionListDictionary[input].Method;
            if (info.GetParameters()[0].ParameterType == typeof(Action))
            {
                _actionListDictionary[input] = Delegate.Combine(value, action);
            }
            else
            {
                Debug.LogError($"Action:{action} is not supported.\n" +
                               $"supported by {info.GetParameters()[0].ParameterType}");
            }
        }
        
        public static void AddAction<T>(InputKind input, Action<T> action)
        {
            _actionListDictionary.TryAdd(input, action);
            var contains = _actionListDictionary.TryGetValue(input, out Delegate value);

            MethodInfo info = _actionListDictionary[input].Method;
            if (info.GetParameters()[0].ParameterType == typeof(Action<T>))
            {
                _actionListDictionary[input] = Delegate.Combine(value, action);
            }
            else
            {
                Debug.LogError($"Action:{action} is not supported.\n" +
                               $"supported by {info.GetParameters()[0].ParameterType}");
            }
        }


        public static void RemoveAction(InputKind input, Action action)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate value))
            {
                _actionListDictionary[input] = Delegate.Remove(value, action);
            }
            else _actionListDictionary.Add(input, action);
        }
        
        public static void RemoveAction<T>(InputKind input, Action<T> action)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate value))
            {
                _actionListDictionary[input] = Delegate.Remove(value, action);
            }
            else _actionListDictionary.Add(input, action);
        }

        private static void Invoke(InputKind input)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate del) && del is Action action)
            {
                action?.Invoke();
            }
        }


        private static void Invoke<T>(InputKind input, T value)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate del) && del is Action<T> action)
            {
                action?.Invoke(value);
            }
        }

        public enum InputKind
        {
            None = 0,
            Move = 2,
            Jump = 1,
        }
    }
}
