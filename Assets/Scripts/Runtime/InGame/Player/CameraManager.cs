using SengokuNinjaVillage.Runtime.System;
using UnityEngine;
using static SengokuNinjaVillage.Runtime.System.InputManager;

namespace SengokuNinjaVillage.Runtime.InGame
{
    public class CameraManager : MonoBehaviour
    {
        Vector2 _inputRotateVector;
        [SerializeField, Header("カメラの横回転速度"), Range(0.1f, 1)] float _rotationSpeedX;
        [SerializeField, Header("カメラの縦回転速度"), Range(0.1f, 1)] float _rotationSpeedY;
        [SerializeField, Header("カメラの縦回転できる限界点")] float _rotationClamp = 30;
        [SerializeField] bool _invertX;
        [SerializeField] bool _invertY;
        [SerializeField] Transform _eye;
        [SerializeField] Transform _player;
        private void OnEnable()
        {
            AddAction<Vector2>(InputKind.Look, InputTriggerType.Performed, OnRotateInput);
            AddAction<Vector2>(InputKind.Look, InputTriggerType.Canceled, OnRotateInput);
        }
        private void OnDisable()
        {
            RemoveAction<Vector2>(InputKind.Look, InputTriggerType.Performed, OnRotateInput);
            RemoveAction<Vector2>(InputKind.Look, InputTriggerType.Canceled, OnRotateInput);
        }

        void Update()
        {
            Rotation(_inputRotateVector);
        }
        void OnRotateInput(Vector2 input)
        {
            _inputRotateVector = input;
        }
        void Rotation(Vector2 input)
        {
            float inputX = input.x;
            float inputY = input.y;

            _player.transform.Rotate(0, inputX * _rotationSpeedX * (_invertX ? -1 : 1), 0);

            var eyeEuler = _eye.localEulerAngles;

            var currentX = eyeEuler.x > 180 ? eyeEuler.x - 360 : eyeEuler.x;

            var newX = currentX + inputY * _rotationSpeedY * (_invertY ? -1 : 1);

            newX = Mathf.Clamp(newX, -_rotationClamp, _rotationClamp);

            _eye.localEulerAngles = new Vector3(newX, eyeEuler.y, eyeEuler.z);
        }

    }
}
