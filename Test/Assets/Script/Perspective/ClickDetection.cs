using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    [SerializeField]
    private Camera _camera = null;

    [SerializeField]
    private float _wheelSpeed = 0f;

    [Header("Info")]
    [SerializeField]
    private GameObject _selectObject = null;

    [SerializeField]
    private float _distance = 0f;

    private Vector3 _direction = Vector3.zero;

    private float _mouseScroll = 0f;

    private float _originalDistance = 0f;

    private float _distanceChange = 0f;

    private float _originalScale = 0f;

    private void Update()
    {
        if (_camera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(this.transform.position, ray.direction, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Fild"))
                {
                    return;
                }

                _selectObject = hit.collider.gameObject;
                _distance = Vector3.Distance(hit.collider.gameObject.transform.position, this.transform.position);
                _originalDistance = _distance;
                _originalScale = _selectObject.transform.localScale.x;

                if(_selectObject.GetComponent<Rigidbody>() == null)
                {
                    return;
                }

                var rigid = _selectObject.GetComponent<Rigidbody>();
                rigid.useGravity = false;
                rigid.freezeRotation = true;
            }
        }

        MouseInput();
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (_selectObject == null)
            {
                return;
            }

            if (_selectObject.GetComponent<Rigidbody>() != null)
            {
                var rigid = _selectObject.GetComponent<Rigidbody>();
                rigid.useGravity = true;
                rigid.freezeRotation = false;
            }

            _selectObject = null;
            _distance = 0;
        }

        if (Input.GetMouseButton(0))
        {
            if (_selectObject == null)
            {
                return;
            }

            _direction = _camera.ScreenPointToRay(Input.mousePosition).direction;

            _mouseScroll = Input.GetAxis("Mouse ScrollWheel") * _wheelSpeed;

            ObjectMove();
        }
    }

    private void ObjectMove()
    {
        _distance += _mouseScroll;

        if (_distance <= 3f)
        {
            _distance = 3f;
        }

        if (_distance >= 30f)
        {
            _distance = 30f;
        }

        _distanceChange = _distance / _originalDistance;
        float viewSclae = _originalScale * _distanceChange;

        _selectObject.transform.localScale = new Vector3(viewSclae, viewSclae, viewSclae);
        _selectObject.transform.position = this.transform.position + (_direction * _distance);
    }
}
