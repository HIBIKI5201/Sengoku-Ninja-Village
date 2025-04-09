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
        [SerializeField] float _jumpPower = 5;
        [SerializeField] float _fallSpeed = 1;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            AddAction(InputKind.Jump, InputTriggerType.Started, Jump);
        }

        void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            RemoveAction(InputKind.Jump, InputTriggerType.Started, Jump);
        }
        private void Update()
        {
            _rb.AddForce(new Vector3(0, -_fallSpeed, 0), ForceMode.Force);
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
            moveVector *= _moveSpeed;
            moveVector.y = _rb.linearVelocity.y;
            _rb.linearVelocity = moveVector;
        }
        void Jump()
        {
            Debug.Log("a");
            _rb.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
        }
    }
}
