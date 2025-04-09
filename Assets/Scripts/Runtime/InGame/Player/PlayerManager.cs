using UnityEngine;
using static SengokuNinjaVillage.Runtime.System.InputManager;

namespace SengokuNinjaVillage
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerManager : MonoBehaviour
    {
        Rigidbody _rb;
        Vector2 _inputVector;
        [SerializeField] float _moveSpeed = 5;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
        }

        void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
        }
        private void Update()
        {
            Move(_inputVector);
        }

        void OnMoveInput(Vector2 input)
        {
            Debug.Log($"input : {input}");

            _inputVector = input;
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
