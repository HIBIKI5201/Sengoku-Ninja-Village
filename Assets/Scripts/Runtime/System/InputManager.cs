using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage.Runtime.System
{
    public static class InputManager // TODO:HERE 可読性が低いので、登録ごとのClassを作成したい
    {
        private static readonly Dictionary<InputKind, Dictionary<InputTriggerType, Delegate>> _actionListDictionary =
            new();

        /// <summary>
        /// Inputに登録されているActionを呼び出すための、Actionを取得するメソッド。
        /// 引数が一致していない場合は、null
        /// </summary>
        /// <param name="input">取得するActionの種類</param>
        /// <param name="type">ボタンからコールバックされるタイミング</param>
        /// <returns>発火用のAction</returns>
        public static Action GetRegisterAction(InputKind input, InputTriggerType type = default)
        {
            _actionListDictionary.TryAdd(input, null);
            if (_actionListDictionary[input] != null && _actionListDictionary[input].GetType() != typeof(Action))
            {
                //引数が登録されたものと一致していなかったらLogを流す。
                Debug.LogWarning($"input:{input} {_actionListDictionary[input].GetType()} is not suported");
                return null;
            }

            return () => Invoke(input, type);
        }

        /// <typeparam name="T">Actionの引数指定</typeparam>
        public static Action GetRegisterAction<T>(InputKind input, InputTriggerType type, T value)
        {
            _actionListDictionary.TryAdd(input, null);
            if (_actionListDictionary[input] != null && _actionListDictionary[input].GetType() != typeof(Action<T>))
            {
                Debug.LogWarning($"{_actionListDictionary[input].GetType()} is not suported");
                return null;
            }

            return () => Invoke(input, type, value);
        }

        /// <summary>
        /// InputごとにActionを登録するメソッド
        /// 引数が前に登録されているものと一致していない場合、ErrorLog
        /// </summary>
        public static void AddAction(InputKind input, InputTriggerType type, Action action)
        {
            if (_actionListDictionary.TryAdd(input, new Dictionary<InputTriggerType, Delegate> {{ type, action }}) || 
                _actionListDictionary[input].TryAdd(type, action)) return; //DicTionaryに登録されていない場合は新しくDictionaryを作成する
            
            if (_actionListDictionary[input].TryGetValue(type, out Delegate value) &&
                value.GetType() == action.GetType())//型が一致している場合はActionを登録する
            {
                _actionListDictionary[input][type] = Delegate.Combine(value, action);
            }
            else //Actionの引数が一致していない場合はError
            {
                Debug.LogError($"Action:{action} is not supported.\n" +
                               $"supported by {_actionListDictionary[input][type].Method.GetParameters()[0].ParameterType}");
            }
        }

        public static void AddAction<T>(InputKind input, InputTriggerType type, Action<T> action)
        {
            if (_actionListDictionary.TryAdd(input, new Dictionary<InputTriggerType, Delegate> {{ type, action }}) || 
                _actionListDictionary[input].TryAdd(type, action)) return;
            
                if (_actionListDictionary[input].TryGetValue(type, out Delegate value) &&
                    value.GetType() == action.GetType())
                {
                    _actionListDictionary[input][type] = Delegate.Combine(value, action);
                }
                else
                {
                    Debug.LogError($"Action:{action} is not supported.\n" +
                                   $"supported by {_actionListDictionary[input][type].Method.GetParameters()[0].ParameterType}");
                }
        }

        /// <summary>
        /// 登録されているActionをRemoveする処理。
        /// Actionの型が一致していない場合、何も起こらない
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type">ボタンからコールバックされるタイミング</param>
        /// <param name="action"></param>
        public static void RemoveAction(InputKind input,　InputTriggerType type, Action action)
        {
            if (_actionListDictionary.ContainsKey(input)&&_actionListDictionary[input].TryGetValue(type, out Delegate value) &&
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input][type] = Delegate.Remove(value, action);
            }
        }

        public static void RemoveAction<T>(InputKind input, InputTriggerType type, Action<T> action)
        {
            if (_actionListDictionary.ContainsKey(input)&&_actionListDictionary[input].TryGetValue(type, out Delegate value) &&
                _actionListDictionary[input].GetType() == action.GetType())
            {
                _actionListDictionary[input][type] = Delegate.Remove(value, action);
            }
        }

        /// <summary>
        /// GetRegisterActionで戻り値が常にActionを参照するようにするためのメソッド
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type">ボタンからコールバックされるタイミング</param>
        private static void Invoke(InputKind input, InputTriggerType type = default)
        {
            //すでに登録されているActionがあり、引数の型が一致している場合の処理
            var isGet = _actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> typeDic);
            if (isGet && typeDic[type] is Action action)
            {
                action?.Invoke();
            }
            else if (isGet && typeDic[type] != null)
            {
                //引数の型が一致しない場合の処理
                var list = typeDic[type].GetInvocationList();
                if (list.Length > 0)
                {
                    var paramType = list[0].Method.GetParameters().FirstOrDefault()?.ParameterType;
                    Debug.Log($"Action<> is not supported." +
                              $" Supported type: {paramType}");
                }
            }
            else
            {
                //Actionが登録されていない場合の処理
                Debug.Log($"{input}Action is null   ");
            }
        }

        private static void Invoke<T>(InputKind input, InputTriggerType type = default, T value = default)
        {
            var isGet = _actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> typeDic);
            if (isGet && typeDic[type] is Action<T> action)
            {
                action?.Invoke(value);
            }
            else if (isGet && typeDic[type] != null)
            {
                var list = typeDic[type].GetInvocationList();
                if (list.Length > 0)
                {
                    var paramType = list[0].Method.GetParameters().FirstOrDefault()?.ParameterType;
                    Debug.LogWarning($"Action:{value.GetType()} is not supported." +
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

        public enum InputTriggerType
        {
            Performed,
            Canceled,
            Started
        }
    }
}