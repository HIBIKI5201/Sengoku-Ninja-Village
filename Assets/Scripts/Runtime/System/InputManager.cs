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
            //Dictionaryの中身が存在するかどうかを確認する。
            if (!_actionListDictionary.TryGetValue(input, out var triggerMap) ||
                !triggerMap.TryGetValue(type, out var del))
            {
                //Debug.LogWarning($"Action for input:{input} and type:{type} is not registered.");
                return null;
            }
            
            if(del is not Action)
            {
                var paramTypeName = del?.Method.GetParameters().FirstOrDefault()?.ParameterType?.Name ?? "Null";
                Debug.LogError($"input:{input} has unsupported delegate type. Expected: Action. Found: {paramTypeName}");
                return null;
            }

            return () => Invoke(input, type);
        }

        /// <typeparam name="T">Actionの引数指定</typeparam>
        public static Action GetRegisterAction<T>(InputKind input, InputTriggerType type, T value)
        {
            if (!_actionListDictionary.TryGetValue(input, out var triggerMap) ||
                !triggerMap.TryGetValue(type, out var del))
            {
                //Debug.LogWarning($"Action for input:{input} and type:{type} is not registered.");
                return null;
            }
            
            if(del is not Action<T>)
            {
                var paramTypeName = del?.Method.GetParameters().FirstOrDefault()?.ParameterType?.Name ?? "Null";
                Debug.LogError($"input:{input} has unsupported delegate type. Expected: Action. Found: {paramTypeName}");
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
            //多重Dictionaryが存在しない場合の処理。
            if (!_actionListDictionary.TryGetValue(input, out var actionList))
            {
                _actionListDictionary[input] = new Dictionary<InputTriggerType, Delegate>()
                {
                    [type] = action
                };
                return;
            }

            //Dictionary内に指定のKeyがない。もしくはDelegateが存在しない場合の処理。
            if (!actionList.TryGetValue(type, out var existing) || existing == null)
            {
                actionList[type] = action;
                return;
            }
            
            //Delegateの型が一致している場合の処理。
            if (existing is Action)
            {
                actionList[type] = Delegate.Combine(existing, action);
                return;
            }

            //Delegateの型が一致していない場合はLogで型の内容を知らせる。
            Debug.LogError($"Action:{action} is not supported.\n" +
                           $"supported by {_actionListDictionary[input][type].Method.GetParameters()[0].ParameterType}");
        }

        public static void AddAction<T>(InputKind input, InputTriggerType type, Action<T> action)
        {
            if (!_actionListDictionary.TryGetValue(input, out var actionList))
            {
                _actionListDictionary[input] = new Dictionary<InputTriggerType, Delegate>()
                {
                    [type] = action
                };
                return;
            }

            if (!actionList.TryGetValue(type, out var existing))
            {
                actionList[type] = action;
                return;
            }

            if (existing is Action<T>)
            {
                actionList[type] = Delegate.Combine(existing, action);
                return;
            }

            Debug.LogError($"Action:{action} is not supported.\n" +
                           $"supported by {_actionListDictionary[input][type].Method.GetParameters()[0].ParameterType}");
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
            //Dictionaryが存在するかの確認
            if (!_actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> actionList) ||
                !actionList.TryGetValue(type, out Delegate del))
            {
                return;
            }

            if (del is Action)//キャストできる場合はRemove
            {
                del = Delegate.Remove(del, action);
            }
            else if (del != null && del.GetType() != typeof(Action))//できない場合はエラーを出す。
            {
                Debug.LogError($"Action:{input} Type:{type} is not supported." +
                          $" Supported type: {del.Method.GetParameters()[0].ParameterType}");
            }
        }

        public static void RemoveAction<T>(InputKind input, InputTriggerType type, Action<T> action)
        {
            if (!_actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> actionList) ||
                !actionList.TryGetValue(type, out Delegate del))
            {
                return;
            }

            if (del is Action<T>)
            {
                del = Delegate.Remove(del, action);
            }
            else if (del != null && del.GetType() != typeof(Action<T>))
            {
                Debug.LogError($"Action:{input} Type:{type} is not supported." +
                          $" Supported type: {del.Method.GetParameters()[0].ParameterType}");
            }
        }

        /// <summary>
        /// GetRegisterActionで戻り値が常にActionを参照するようにするためのメソッド
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type">ボタンからコールバックされるタイミング</param>
        private static void Invoke(InputKind input, InputTriggerType type = default)
        {
            //Dictionaryが存在しない場合の例外処理
            if (!_actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> actionList) ||
                !actionList.TryGetValue(type, out Delegate del))
            {
                Debug.LogError($"Action:{input} Type:{type} is null");
                return;
            }

            //登録されているDelegateがActionにキャストできる場合の処理。
            if (del is Action action)
            {
                action?.Invoke();
            }
            else if (del == null) //delegateが存在しない場合の処理。
            {
                Debug.LogError($"Action:{input} Type:{type} is null");
            }
            else //delegateの型が一致していない場合の処理
            {
                Debug.LogError($"Action:{input} Type:{type} is not supported." +
                          $" Supported type: {del.Method.GetParameters()[0].ParameterType}");
            }
        }

        private static void Invoke<T>(InputKind input, InputTriggerType type = default, T value = default)
        {
            //Dictionaryが存在しない場合の例外処理
            if (!_actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> actionList) ||
                !_actionListDictionary[input].TryGetValue(type, out Delegate del))
            {
                Debug.LogError($"Action:{input} Type:{type} is null");
                return;
            }

            //登録されているDelegateがActionにキャストできる場合の処理。
            if (del is Action<T> action)
            {
                action?.Invoke(value);
            }
            else if (del == null) //delegateが存在しない場合の処理。
            {
                Debug.LogError($"Action:{input} Type:{type} is null");
            }
            else //delegateの型が一致していない場合の処理
            {
                Debug.LogError($"Action:{input} Type:{type} is not supported." +
                          $" Supported type: {del.Method.GetParameters()[0].ParameterType}");
            }
        }

        /// <summary>
        /// 登録されているActionの引数をLogに出力する、デバッグ用のメソッド
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        public static void GetParameterType(InputKind input, InputTriggerType type)
        {
            if (!_actionListDictionary.TryGetValue(input, out Dictionary<InputTriggerType, Delegate> actionList) ||
                !actionList.TryGetValue(type, out Delegate del))
            {
                Debug.Log($"input:{input} Type:{type} Dictionary is null");
                return;
            }
            var paramTypeName = del?.Method.GetParameters().FirstOrDefault()?.ParameterType?.Name ?? "Null";
            Debug.LogError($"input:{input} Type:{type} Expected: Action. Found: {paramTypeName}");
            return;
        }

        /// EnterPlayMode用
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetActions()
        {
            Debug.Log($"{_actionListDictionary} Reset");
            _actionListDictionary.Clear();
        }

        public enum InputTriggerType
        {
            Performed,
            Canceled,
            Started
        }
    }
}