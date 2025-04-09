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
        [SerializeField] Transform _underfoot;
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] CapsuleCollider _collider;
        bool _isCrouched;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            AddAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            AddAction(InputKind.Jump, InputTriggerType.Started, Jump);
            AddAction(InputKind.Crouch, InputTriggerType.Started, Crouch);
        }

        void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            RemoveAction(InputKind.Jump, InputTriggerType.Started, Jump);
            RemoveAction(InputKind.Crouch, InputTriggerType.Started, Crouch);
        }
        private void Update()
        {
            _rb.AddForce(new Vector3(0, -_fallSpeed, 0), ForceMode.Force);
            Move(_inputVector);
        }

        void OnMoveInput(Vector2 input)
        {
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
            moveVector *= _isCrouched ? _moveSpeed * 0.2f : _moveSpeed;
            moveVector.y = _rb.linearVelocity.y;
            _rb.linearVelocity = moveVector;
        }
        void Jump()
        {
            if (Physics.Raycast(_underfoot.position, Vector3.down, 0.1f, _groundLayer))
            {
                _rb.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            }
        }
        void Crouch()
        {
            _isCrouched = !_isCrouched;
            var center = _collider.center;
            if (_isCrouched)
            {
                center.y /= 2;
                _collider.center = center;
                _collider.height /= 2;
                return;
            }
            center = _collider.center;
            center.y *= 2;
            _collider.center = center;
            _collider.height *= 2;
        }
    }
}
