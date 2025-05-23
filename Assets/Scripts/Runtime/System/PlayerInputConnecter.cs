using System;
using System.Collections.Generic;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage.Runtime.System
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputConecter : MonoBehaviour
    {
        private PlayerInput _inputSystem;

        private List<(Action<InputAction.CallbackContext> action, string name, InputManager.InputTriggerType type)>
            _registrationAction = new();

        private static readonly Dictionary<string, InputKind> InputDictionary =
            new()
            {
                { "Move", InputKind.Move },
                { "Jump", InputKind.Jump },
                { "Dash", InputKind.Dash },
                { "Crouch", InputKind.Crouch },
                { "Interact", InputKind.Interact },
                {"Look", InputKind.Look },
                {"Button1", InputKind.Button1 },
                {"Button2", InputKind.Button2 },
                {"Button3", InputKind.Button3 },
            };

        private void Awake()
        {
            ServiceLocator.SetInstance(this, ServiceLocator.LocateType.Singleton);
        }

        private void Start()
        {
            _inputSystem = GetComponent<PlayerInput>();

            if (_inputSystem == null)
            {
                return;
            }

            BindDefaultActions("Jump");
            BindDefaultActions("Dash");
            BindDefaultActions("Crouch");
            BindDefaultActions("Interact");

            BindVector2Actions("Move");
            BindVector2Actions("Look");
        }

        private void OnDestroy()
        {
            foreach (var removeData in _registrationAction)
            {
                switch (removeData.type)
                {
                    case InputManager.InputTriggerType.Canceled:
                        _inputSystem.actions[removeData.name].canceled -= removeData.action;
                        break;
                    case InputManager.InputTriggerType.Performed:
                        _inputSystem.actions[removeData.name].performed -= removeData.action;
                        break;
                    case InputManager.InputTriggerType.Started: 
                        _inputSystem.actions[removeData.name].started -= removeData.action;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            ServiceLocator.DestroyInstance<PlayerInputConecter>();
        }

        /// <summary>
        /// 引数が不要なActionの登録メソッド
        /// </summary>
        /// <param name="actionName"></param>
        private void BindDefaultActions(string actionName)
        {
            //一時的にActionを変数に格納
            Action<InputAction.CallbackContext> action = _ =>
                InputManager.GetRegisterAction(InputDictionary[actionName], InputManager.InputTriggerType.Performed)
                    ?.Invoke();
            _inputSystem.actions[actionName].performed += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Performed)); //ListにAction登録

            action = _ =>
                InputManager.GetRegisterAction(InputDictionary[actionName], InputManager.InputTriggerType.Canceled)?.Invoke();
            _inputSystem.actions[actionName].canceled += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Canceled));

            action = _ =>
                InputManager.GetRegisterAction(InputDictionary[actionName], InputManager.InputTriggerType.Started)?.Invoke();
            _inputSystem.actions[actionName].started += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Started));
        }

        /// <summary>
        /// Vector2のAction登録用メソッド
        /// </summary>
        /// <param name="actionName"></param>
        private void BindVector2Actions(string actionName)
        {
            Action<InputAction.CallbackContext> action = x => InputManager.GetRegisterAction(InputDictionary[actionName],
                InputManager.InputTriggerType.Performed,
                x.ReadValue<Vector2>())?.Invoke();
            _inputSystem.actions[actionName].performed += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Performed));

            action = x => InputManager.GetRegisterAction(InputDictionary[actionName],
                InputManager.InputTriggerType.Canceled,
                x.ReadValue<Vector2>())?.Invoke();
            _inputSystem.actions[actionName].canceled += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Canceled));

            action = x => InputManager.GetRegisterAction(InputDictionary[actionName],
                InputManager.InputTriggerType.Started,
                x.ReadValue<Vector2>())?.Invoke();
            _inputSystem.actions[actionName].started += action;
            _registrationAction.Add((action, actionName, InputManager.InputTriggerType.Started));
        }

        /// <summary>
        /// デバッグ用のAction inspectorから追加可能
        /// </summary>
        [ContextMenu("DebugActions")]
        private void DebugActions()
        {
            Debug.Log("Debug用のAction追加");
            InputManager.AddAction<Vector2>(InputKind.Move, InputManager.InputTriggerType.Performed,
                x => Debug.Log(x));
            InputManager.AddAction<Vector2>(InputKind.Move, InputManager.InputTriggerType.Canceled,
                x => Debug.Log(x));
            InputManager.AddAction<Vector2>(InputKind.Move, InputManager.InputTriggerType.Started,
                x => Debug.Log(x));

            InputManager.AddAction(InputKind.Jump, InputManager.InputTriggerType.Canceled,
                () => Debug.Log("Jump`"));
            InputManager.AddAction(InputKind.Jump, InputManager.InputTriggerType.Started,
                () => Debug.Log("Jump!"));
            InputManager.AddAction(InputKind.Jump, InputManager.InputTriggerType.Performed,
                () => Debug.Log("Jump?"));
            
            InputManager.AddAction<Vector2>(InputKind.Look, InputManager.InputTriggerType.Performed,
                x => Debug.Log(x));

            _inputSystem.actions["Move"].performed += _ => Debug.Log("Move");
        }
    }
}