using System;
using SengokuNinjaVillage.Runtime.System;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage
{
    public class PlayerInputConecter : MonoBehaviour
    {
        private PlayerInput inputSystem;

        private void Awake()
        {
            ServiceLocator.SetInstance(this, ServiceLocator.LocateType.Singleton);
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            inputSystem = GetComponent<PlayerInput>();
            
            if (inputSystem == null)
            {
                return;
            }
            inputSystem.actions["Move"].performed += OnMoving;
            inputSystem.actions["Jump"].performed += OnJump;
            inputSystem.actions["Dash"].performed += OnDash;
            inputSystem.actions["Crouch"].performed += OnCrouch;
        }

        private void OnDestroy()
        {
            inputSystem.actions["Move"].performed -= OnMoving;
            ServiceLocator.DestroyInstance<PlayerInputConecter>();
        }

        private void OnMoving(InputAction.CallbackContext contextMenu)
        {
            var n = contextMenu.ReadValue<Vector2>();
            InputManager.GetRegisterAction(InputManager.InputKind.Move, n)?.Invoke();
        }
        private void OnDash(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Dash)?.Invoke();
        }
        private void OnJump(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Jump)?.Invoke();
        }
        private void OnCrouch(InputAction.CallbackContext contextMenu)
        {
            InputManager.GetRegisterAction(InputManager.InputKind.Crouch)?.Invoke();
        }
    }
}