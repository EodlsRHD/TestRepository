using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MinecraftCam
{
    public class MinecraftCam : MonoBehaviour
    {
        enum eMode
        {
            hold,
            hover,
            autoUp,
            breathing
        }

        [Header("Camera")]
        [SerializeField]
        [Range(50f, 10f)]
        private float _cameraUpAngle = 45f;

        [SerializeField]
        [Range(320f, 360f)]
        private float _cameraDownAngle = 340f;

        [SerializeField]
        [Range(0f, 10f)]
        private float _cameraSensitive = 2f;

        [Header("Movement")]
        [SerializeField]
        [Range(0f, 10f)]
        private float _maxPower = 7f;

        private float _movePower = 0f;

        private float _flyPower = 0f;

        [SerializeField]
        [Range(0f, 0.5f)]
        private float _deceleration = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _acceleration = 0.8f;

        [Header("Mode")]
        [SerializeField]
        private eMode _mode = eMode.hover;

        [SerializeField]
        [Range(0f, 0.1f)]
        private float _autoUpSpeed = 0.02f;

        [SerializeField]
        [Range(0f, 0.1f)]
        private float _breathingSpeed = 0.02f;

        [SerializeField]
        [Range(0f, 10f)]
        private float _breathingTime = 3f;

        [SerializeField]
        private float _distanceFromTheFloor = 2f;

        [SerializeField]
        [Range(0.001f, 0.1f)]
        private float _gravity = 0.01f;

        private float _breathTime = 0f;


        private Vector3 _flyDir = Vector3.zero;

        private Vector3 _moveDir = Vector3.zero;

        private bool _isMove = false;

        private bool _isFly = false;

        private void Update()
        {
            Vector3 pos = this.transform.position;

            if (_mode == eMode.hold)
            {
                HoldMode(pos);
            }
            UpdateControll();

            Moving(pos, _movePower, _flyPower, _moveDir);
        }

        void HoldMode(Vector3 pos)
        {
            RaycastHit hit;

            if (Physics.Raycast(pos, Vector3.down, out hit, _distanceFromTheFloor))
            {
                float dis = Mathf.Round(Mathf.Abs(this.transform.position.y - hit.point.y) * 10) * 0.1f;
                if (dis < _distanceFromTheFloor)
                {
                    pos.y += _distanceFromTheFloor - dis;
                    this.transform.position = pos;
                }
                return;
            }

            pos.y -= _gravity;
            this.transform.position = pos;
        }

        private void UpdateControll()
        {
            ViewDetect();
            MoveDetect();

            if (_mode == eMode.hold)
            {
                return;
            }

            UpAndDown(KeyCode.Space, Vector3.up);
            UpAndDown(KeyCode.LeftShift, Vector3.down);
        }

    private void Moving(Vector3 pos, float movePower, float flyPower, Vector3 moveDir)
    {
        RaycastHit Downhit;
        if (Physics.Raycast(pos, Vector3.down, out Downhit, _distanceFromTheFloor))
        {
            float dis = Mathf.Round(Mathf.Abs(this.transform.position.y - Downhit.point.y) * 10) * 0.1f;
            if (dis < _distanceFromTheFloor)
            {
                pos.y += _distanceFromTheFloor - dis;
            }
        }

        Vector3 newDir = (moveDir + _flyDir).normalized;
        float power = (movePower + flyPower);

        RaycastHit MoveHit;
        if (Physics.Raycast(pos, newDir, out MoveHit, _distanceFromTheFloor))
        {
            newDir = Vector3.Reflect(newDir, MoveHit.normal).normalized;

            if (Vector3.Angle(newDir, MoveHit.normal) == 0)
            {
                return;
            }

            RaycastHit sechit;
            if (Physics.Raycast(MoveHit.point, newDir, out sechit, _distanceFromTheFloor))
            {
                newDir = Vector3.Reflect(newDir, sechit.normal).normalized;
            }
        }

        this.transform.position = Vector3.Lerp(this.transform.position, pos + newDir, power * Time.deltaTime);
    }

        void ViewDetect()
        {
            if (Input.GetMouseButton(1) == false)
            {
                return;
            }

            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * _cameraSensitive, Input.GetAxisRaw("Mouse Y") * _cameraSensitive);
            Vector3 camAngle = this.transform.eulerAngles;
            float x = camAngle.x - mouseDelta.y;
            float y = camAngle.y + mouseDelta.x;

            if (x < 180f)
            {
                x = Mathf.Clamp(x, -1f, _cameraUpAngle);
            }
            if (x > 180f)
            {
                x = Mathf.Clamp(x, _cameraDownAngle, 361f);
            }

            this.transform.rotation = Quaternion.Euler(x, y, camAngle.z);
        }

        private void MoveDetect()
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                _isMove = false;
                return;
            }

            _isMove = true;
            _movePower = _maxPower;

            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 lookForward = this.transform.forward.normalized;
            Vector3 lookRight = this.transform.right.normalized;

            _moveDir = ((lookForward * moveInput.y) + (lookRight * moveInput.x)).normalized;
        }

        private void UpAndDown(KeyCode keycode, Vector3 dir)
        {
            if (Input.GetKey(keycode) == false)
            {
                _isFly = false;
                return;
            }

            _isFly = true;
            _flyPower = _maxPower;
            _flyDir = dir;
        }

        private void FixedUpdate()
        {
            if (_isMove == false)
            {
                MoveDeceleration();
            }

            if (_isFly == false)
            {
                FlyDeceleration();
            }

            if (_isMove == false && _isFly == false)
            {
                Mode(_mode);
            }
        }

        void MoveDeceleration()
        {
            if (_movePower <= 0f)
            {
                _moveDir = Vector3.zero;
                _movePower = 0f;
                return;
            }
            _movePower -= _deceleration;
        }

        void FlyDeceleration()
        {
            if (_flyPower <= 0f)
            {
                _flyDir = Vector3.zero;
                _flyPower = 0f;
                return;
            }
            _flyPower -= _deceleration;
        }

        void Mode(eMode mode)
        {
            switch (mode)
            {
                case eMode.autoUp:
                    Moving(this.transform.position, _breathingSpeed * 10, 0f, Vector3.up);
                    break;

                case eMode.breathing:
                    _breathTime += Time.deltaTime;

                    if (_breathTime <= _breathingTime)
                    {
                        Moving(this.transform.position, _breathingSpeed * 10, 0f, Vector3.up);
                    }
                    if (_breathingTime < _breathTime && _breathTime <= (_breathingTime * 2))
                    {
                        Moving(this.transform.position, _breathingSpeed * 10, 0f, Vector3.down);
                    }

                    if (_breathTime > (_breathingTime * 2))
                    {
                        _breathTime = 0f;
                    }
                    break;
            }
        }
    }
}