using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage.Runtime.System
{
    public static class InputManager
    {
        
        private static readonly Dictionary<InputKind, Delegate> _actionListDictionary = new ();
        
        public static Action GetRegisterAction(InputKind input)
        {
            _actionListDictionary.TryAdd(input, null);
            return () => Invoke(input);
        }
        
        public static Action GetRegisterAction<T>(InputKind input, T value)
        {
            _actionListDictionary.TryAdd(input, null);
            return () => Invoke(input, value);
        }

        public static void AddAction(InputKind input, Action action)
        {
            if(_actionListDictionary.TryAdd(input, action)) return;
            if (_actionListDictionary.TryGetValue(input, out Delegate value) &&
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input] = Delegate.Combine(value, action);
            }
            else
            {
                Debug.LogError($"Action:{action} is not supported.\n" + 
                               $"supported by {_actionListDictionary[input].Method.GetParameters()[0].ParameterType}");
            }
        }
        
        public static void AddAction<T>(InputKind input, Action<T> action)
        {
            if(_actionListDictionary.TryAdd(input, action)) return;
            if (_actionListDictionary.TryGetValue(input, out Delegate value) &&
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input] = Delegate.Combine(value, action);
            }
            else
            {
                Debug.LogError($"Action:{action} is not supported.\n" + 
                               $"supported by {_actionListDictionary[input].Method.GetParameters()[0].ParameterType}");
            }
        }


        public static void RemoveAction(InputKind input, Action action)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate value) && 
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input] = Delegate.Remove(value, action);
            }
        }
        
        public static void RemoveAction<T>(InputKind input, Action<T> action)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate value)&& 
            _actionListDictionary[input].GetType() == action.GetType())
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
            else if (_actionListDictionary.TryGetValue(input, out var fallback) && fallback != null)
            {
                var list = fallback.GetInvocationList();
                if (list.Length > 0)
                {
                    var paramType = list[0].Method.GetParameters().FirstOrDefault()?.ParameterType;
                    Debug.Log($"Action<> is not supported." +
                              $" Supported type: {paramType}");
                }
            }
            else
            {
                Debug.Log($"{input}Action is null   ");
            }
            
        }

        private static void Invoke<T>(InputKind input, T value)
        {
            if (_actionListDictionary.TryGetValue(input, out Delegate del) && del is Action<T> action)
            {
                Debug.Log(del as Action<T>);
                action?.Invoke(value);
            }
            else if (_actionListDictionary.TryGetValue(input, out var fallback) && fallback != null)
            {
                var list = fallback.GetInvocationList();
                if (list.Length > 0)
                {
                    var paramType = list[0].Method.GetParameters().FirstOrDefault()?.ParameterType;
                    Debug.Log($"Action:{value.GetType()} is not supported." +
                              $" Supported type: {paramType}");
                }
            }
            else
            {
                Debug.Log($"{input}Action is null   ");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetActions()
        {
            Debug.Log($"{_actionListDictionary} Reset");
            _actionListDictionary.Clear();
        }

        public enum InputKind
        {
            None = 0,
            Move = 1,
            Jump = 2,
            Crouch = 3,
            Dash = 4
        }
    }
}
