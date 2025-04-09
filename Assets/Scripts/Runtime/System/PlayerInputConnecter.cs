using System;
using System.Collections.Generic;
using SengokuNinjaVillage.Runtime.System;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage.Runtime.System
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputConecter : MonoBehaviour
    {
        private PlayerInput _inputSystem;

        private static readonly Dictionary<string, InputManager.InputKind> InputKind =
            new()
            {
                { "Move", InputManager.InputKind.Move },
                { "Jump", InputManager.InputKind.Jump },
                { "Dash", InputManager.InputKind.Dash },
                { "Crouch", InputManager.InputKind.Crouch },
                { "Interact", InputManager.InputKind.Interact },
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
            
            Debug.Log("Registered Actions");
        }

        private void OnDestroy()
        {
            //再生時にOnDestroyが呼ばれて、エラーが出たため一時的にTry Catch
            try
            {
                //_inputSystem.actions["Move"].performed -= OnMoving;
            }
            catch (Exception e)
            {
            }
            finally
            {
                ServiceLocator.DestroyInstance<PlayerInputConecter>();
            }
        }


        private void BindDefaultActions(string actionName)
        {
                _inputSystem.actions[actionName].performed +=
                    _ =>
                    {
                        InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Performed)?.Invoke();
                    };
                _inputSystem.actions[actionName].canceled +=
                    _ => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Canceled)?.Invoke();
                _inputSystem.actions[actionName].started +=
                    _ => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Started)?.Invoke();
        }
        private void BindVector2Actions(string actionName)
        {
            try
            {
                _inputSystem.actions[actionName].performed +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Performed, x.ReadValue<Vector2>())?.Invoke();
                _inputSystem.actions[actionName].canceled +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Canceled, x.ReadValue<Vector2>())?.Invoke();
                _inputSystem.actions[actionName].started +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Started, x.ReadValue<Vector2>())?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{actionName} action could not be executed.");
                throw;
            }
        }

        [ContextMenu("DebugActions")]
        private void DebugActions()
        {
            Debug.Log("Debug用のAction追加");
            InputManager.AddAction<Vector2>(InputManager.InputKind.Move, InputManager.InputTriggerType.Performed, x => Debug.Log(x));
            InputManager.AddAction<Vector2>(InputManager.InputKind.Move, InputManager.InputTriggerType.Canceled, x => Debug.Log(x));
            InputManager.AddAction<Vector2>(InputManager.InputKind.Move, InputManager.InputTriggerType.Started, x => Debug.Log(x));
            
            InputManager.AddAction(InputManager.InputKind.Jump, InputManager.InputTriggerType.Canceled, ()=>Debug.Log("Jump`"));
            InputManager.AddAction(InputManager.InputKind.Jump, InputManager.InputTriggerType.Started, ()=> Debug.Log("Jump!"));
            InputManager.AddAction(InputManager.InputKind.Jump, InputManager.InputTriggerType.Performed, () => Debug.Log("Jump?"));
            
            _inputSystem.actions["Move"].performed += _ => Debug.Log("Move");
        }
    }
}