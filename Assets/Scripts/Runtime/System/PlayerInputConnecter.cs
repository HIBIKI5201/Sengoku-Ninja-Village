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
            
            BindDefaultActions("Move");
        }

        private void OnDestroy()
        {
            //再生時にOnDestroyが呼ばれて、エラーが出たため一時的にTry Catch
            try
            {
                _inputSystem.actions["Move"].performed -= OnMoving;
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
            try
            {
                _inputSystem.actions[actionName].performed +=
                    _ => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Performed);
                _inputSystem.actions[actionName].canceled +=
                    _ => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Canceled);
                _inputSystem.actions[actionName].started +=
                    _ => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Started);
            }
            catch (Exception e)
            {
                Debug.LogError($"{actionName} action could not be executed.");
                throw;
            }
        }
        private void BindVector2Actions(string actionName)
        {
            try
            {
                _inputSystem.actions[actionName].performed +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Performed, x.ReadValue<Vector2>());
                _inputSystem.actions[actionName].canceled +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Canceled, x.ReadValue<Vector2>());
                _inputSystem.actions[actionName].started +=
                    x => InputManager.GetRegisterAction(InputKind[actionName], InputManager.InputTriggerType.Started, x.ReadValue<Vector2>());
            }
            catch (Exception e)
            {
                Debug.LogError($"{actionName} action could not be executed.");
                throw;
            }
        }

        private void OnMoving(InputAction.CallbackContext contextMenu)
        {
            var input = contextMenu.ReadValue<Vector2>();
            InputManager.GetRegisterAction(InputManager.InputKind.Move, InputManager.InputTriggerType.Performed, input)
                ?.Invoke();
        }

        private void OnCancelMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            InputManager.GetRegisterAction(InputManager.InputKind.Move, InputManager.InputTriggerType.Canceled, input)
                ?.Invoke();
        }

        private void OnDash(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Dash)?.Invoke();
        }

        private void OnJumping(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Jump)?.Invoke();
        }

        private void OnCrouch(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Crouch)?.Invoke();
        }

        private void OnInteract(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Interact)?.Invoke();
        }
    }
}