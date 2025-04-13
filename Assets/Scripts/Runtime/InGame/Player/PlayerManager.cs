using SengokuNinjaVillage.Runtime.System;
using UnityEngine;
using static SengokuNinjaVillage.Runtime.System.InputManager;

namespace SengokuNinjaVillage
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerManager : MonoBehaviour
    {
        Rigidbody _rb;
        Vector2 _inputMoveVector;
        Vector2 _inputRotateVector;
        [SerializeField, Header("移動速度")] float _moveSpeed = 5;
        [SerializeField, Header("ジャンプ力")] float _jumpPower = 5;
        [SerializeField, Header("落下速度")] float _fallSpeed = 1;
        [SerializeField, Header("ローリング回避する距離")] float _rollingDistance = 1;
        [SerializeField, Header("ローリング回避する時間")] float _rollingTime = 0.5f;
        [SerializeField, Header("カメラの回転速度"), Range(0.1f, 1)] float _rotationSpeed;
        [SerializeField, Header("カメラの回転できる限界点")] float _rotationClamp = 30;
        [SerializeField] Transform _underfoot;
        [SerializeField] Transform _eye;
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
            AddAction<Vector2>(InputKind.Look, InputTriggerType.Performed, OnRotateInput);
            AddAction<Vector2>(InputKind.Look, InputTriggerType.Canceled, OnRotateInput);
            AddAction(InputKind.Jump, InputTriggerType.Started, Jump);
            AddAction(InputKind.Crouch, InputTriggerType.Started, Crouch);
            AddAction(InputKind.Dash, InputTriggerType.Started, Rolling);
            AddAction(InputKind.Dash, InputTriggerType.Canceled, DashEnd);
        }

        void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Performed, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Move, InputTriggerType.Canceled, OnMoveInput);
            RemoveAction<Vector2>(InputKind.Look, InputTriggerType.Performed, OnRotateInput);
            RemoveAction<Vector2>(InputKind.Look, InputTriggerType.Canceled, OnRotateInput);
            RemoveAction(InputKind.Jump, InputTriggerType.Started, Jump);
            RemoveAction(InputKind.Crouch, InputTriggerType.Started, Crouch);
            RemoveAction(InputKind.Dash, InputTriggerType.Started, Rolling);
            RemoveAction(InputKind.Dash, InputTriggerType.Canceled, DashEnd);
        }
        private void Update()
        {
            Rotation(_inputRotateVector);

            _rb.AddForce(new Vector3(0, -_fallSpeed, 0), ForceMode.Force);

            if (!_isRolling)
            {
                Move(_inputMoveVector);
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
            _inputMoveVector = input;
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

        void OnRotateInput(Vector2 input)
        {
            _inputRotateVector = input;
        }
        void Rotation(Vector2 input)
        {
            float inputX = input.x;
            float inputY = input.y;

            transform.Rotate(0, inputX * _rotationSpeed, 0);

            var eyeEuler = _eye.localEulerAngles;

            var currentX = eyeEuler.x > 180 ? eyeEuler.x - 360 : eyeEuler.x;

            var newX = currentX + inputY * _rotationSpeed;

            newX = Mathf.Clamp(newX, -_rotationClamp, _rotationClamp);

            _eye.localEulerAngles = new Vector3(newX, eyeEuler.y, eyeEuler.z);
        }
        void ColliderSizeChange(float value)
        {
            _collider.center *= value;
            _collider.height *= value;
        }
    }
}
