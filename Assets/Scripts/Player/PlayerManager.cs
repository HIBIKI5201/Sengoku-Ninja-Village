using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] InputAction _input;
        Vector2 _inputVector;
        void OnEnable()
        {
            _input.Enable();
            _input.started += Input;
            _input.performed += Input;
            _input.canceled += Input;
        }

        void OnDisable()
        {
            _input.started -= Input;
            _input.performed -= Input;
            _input.canceled -= Input;
            _input.Disable();
        }
        private void Update()
        {
            Move(_inputVector);
        }

        void Input(InputAction.CallbackContext context)
        {
            _inputVector = _input.ReadValue<Vector2>();
        }

        void Move(Vector2 input)
        {
            var forward = Camera.main.transform.forward;
            var right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            var moveVector = forward * input.y + right * input.x;
            transform.position += moveVector * 2f * Time.deltaTime;
        }
    }
}
