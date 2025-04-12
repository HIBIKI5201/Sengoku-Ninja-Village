using UnityEngine;
using static SengokuNinjaVillage.Runtime.System.InputManager;

namespace SengokuNinjaVillage
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerManager : MonoBehaviour
    {
        Rigidbody _rb;
        Vector2 _inputVector;
        [SerializeField, Header("移動速度")] float _moveSpeed = 5;
        [SerializeField, Header("ジャンプ力")] float _jumpPower = 5;
        [SerializeField, Header("落下速度")] float _fallSpeed = 1;
        [SerializeField, Header("ローリング回避する距離")] float _rollingDistance = 1;
        [SerializeField, Header("ローリング回避する時間")] float _rollingTime = 0.5f;
        [SerializeField] Transform _underfoot;
        [SerializeField] LayerMask _groundLayer;
        [SerializeField] CapsuleCollider _collider;

        float _rollingTimer;

        bool _isCrouched;
        bool _isDash;
        bool _isRolling;

        Vector3 _moveDir;
        Vector3 _startPos;
        Vector3 _targetPos;

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
            AddAction(InputKind.Dash, InputTriggerType.Started, Rolling);
            AddAction(InputKind.Dash, InputTriggerType.Canceled, DashEnd);
        }

        void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            RemoveAction(InputKind.Jump, InputTriggerType.Started, Jump);
            RemoveAction(InputKind.Crouch, InputTriggerType.Started, Crouch);
            RemoveAction(InputKind.Dash, InputTriggerType.Started, Rolling);
            RemoveAction(InputKind.Dash, InputTriggerType.Canceled, DashEnd);
        }
        private void Update()
        {
            var camForward = Camera.main.transform.forward;
            camForward.y = 0;
            transform.forward = camForward;

            _rb.AddForce(new Vector3(0, -_fallSpeed, 0), ForceMode.Force);

            if (!_isRolling)
            {
                Move(_inputVector);
            }

            else if (_isRolling)
            {
                _rollingTimer += Time.deltaTime;

                float t = Mathf.Clamp01(_rollingTimer / _rollingTime);
                transform.position = Vector3.Lerp(_startPos, _targetPos, t);
                Debug.Log(t);
                if (t >= 1)
                {
                    _isRolling = false;
                    ColliderSizeChange(2);
                }
            }

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

            _moveDir = forward * input.y + right * input.x;
            var moveVector = _moveDir * (_isDash ? _moveSpeed * 1.5f : _isCrouched ? _moveSpeed * 0.2f : _moveSpeed);
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
                ColliderSizeChange(0.5f);
                return;
            }
            ColliderSizeChange(2);
        }
        void Rolling()
        {
            _isDash = true;
            if (!_isRolling)
            {
                ColliderSizeChange(0.5f);
                _isRolling = true;
                _rollingTimer = 0;
                _startPos = transform.position;
                _targetPos = transform.position + (_moveDir == Vector3.zero ? transform.forward : _moveDir) * _rollingDistance;
            }
        }
        void DashEnd()
        {
            _isDash = false;
        }
        void ColliderSizeChange(float value)
        {
            _collider.center *= value;
            _collider.height *= value;
        }
    }
}
