using System;
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
            _inputSystem.actions["Move"].performed += OnMoving;
            _inputSystem.actions["Jump"].performed += OnJumping;
            _inputSystem.actions["Dash"].performed += OnDash;
            _inputSystem.actions["Crouch"].performed += OnCrouch;
            _inputSystem.actions["Interact"].performed += OnInteract;
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

        private void OnMoving(InputAction.CallbackContext contextMenu)
        {
            var input = contextMenu.ReadValue<Vector2>();
            InputManager.GetRegisterAction(InputManager.InputKind.Move, input)?.Invoke();
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