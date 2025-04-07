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
        
        /// <summary>
        /// Inputに登録されているActionを呼び出すための、Actionを取得するメソッド。
        /// 引数が一致していない場合は、null
        /// </summary>
        /// <param name="input">取得するActionの種類</param>
        /// <returns>発火用のAction</returns>
        public static Action GetRegisterAction(InputKind input)
        {
            _actionListDictionary.TryAdd(input, null);
            if ( _actionListDictionary[input] != null && _actionListDictionary[input].GetType() != typeof(Action))
            {
                //引数が登録されたものと一致していなかったらLogを流す。
                Debug.Log(_actionListDictionary[input].GetType());
                return null;
            }
            
            return () => Invoke(input);
        }
        /// <typeparam name="T">Actionの引数指定</typeparam>
        public static Action GetRegisterAction<T>(InputKind input, T value)
        {
            _actionListDictionary.TryAdd(input, null);
            if ( _actionListDictionary[input] != null && _actionListDictionary[input].GetType() != typeof(Action<T>))
            {
                Debug.Log(_actionListDictionary[input].GetType());
                return null;
            }
            return () => Invoke(input, value);
        }

        /// <summary>
        /// InputごとにActionを登録するメソッド
        /// 引数が前に登録されているものと一致していない場合、ErrorLog
        /// </summary>
        public static void AddAction(InputKind input, Action action)
        {
            if(_actionListDictionary.TryAdd(input, action)) return;//DicTionaryに登録されていない場合の処理
            if (_actionListDictionary.TryGetValue(input, out Delegate value) &&　
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input] = Delegate.Combine(value, action);
            }
            else        //Actionの引数が一致していない場合の処理
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

        /// <summary>
        /// 登録されているActionをRemoveする処理。
        /// Actionの型が一致していない場合、何も起こらない
        /// </summary>
        /// <param name="input"></param>
        /// <param name="action"></param>
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
        }

        /// <summary>
        /// GetRegisterActionで戻り値が常にActionを参照するようにするためのメソッド
        /// </summary>
        /// <param name="input"></param>
        private static void Invoke(InputKind input)
        {
            //すでに登録されているActionがあり、引数の型が一致している場合の処理
            if (_actionListDictionary.TryGetValue(input, out Delegate del) && del is Action action)
            {
                action?.Invoke();
            }
            else if (_actionListDictionary.TryGetValue(input, out var fallback) && fallback != null)
            {
                //引数の型が一致しない場合の処理
                var list = fallback.GetInvocationList();
                if (list.Length > 0)
                {
                    var paramType = list[0].Method.GetParameters().FirstOrDefault()?.ParameterType;
                    Debug.Log($"Action<> is not supported." +
                              $" Supported type: {paramType}");
                }
            }
            else 
            {   //Actionが登録されていない場合の処理
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
        
        /// EnterPlayMode用
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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
            Dash = 4,
            Interact = 5
        }
    }
}
