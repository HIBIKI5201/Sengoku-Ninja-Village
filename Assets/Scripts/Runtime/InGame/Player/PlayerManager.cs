using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SengokuNinjaVillage
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] InputAction _input;
        Rigidbody _rb;
        Vector2 _inputVector;
        float _moveSpeed = 5;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
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
            _rb.linearVelocity = moveVector * _moveSpeed;
        }
    }
}
