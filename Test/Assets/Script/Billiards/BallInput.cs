using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallInput : MonoBehaviour
{
    [SerializeField]
    private Image _scrollbar = null;

    [SerializeField]
    private Ball _ball = null;

    [SerializeField]
    private GameObject _queueStick = null;

    [SerializeField]
    private GameObject _queueStickCenteralAxis = null;

    private int layerMask = 0;

    private float _dis = 0f;

    private Vector3 _dir = Vector3.zero;

    private Vector3 _posiion = Vector3.zero;

    private float _powerGauge = 0f;

    private void Awake()
    {
        layerMask = 1 << LayerMask.NameToLayer("InputFild");
    }

    private void Update()
    {
        if (_ball.IsMove == true)
        {
            return;
        }

        _queueStickCenteralAxis.transform.position = _ball.transform.position;

        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(ZeroPowerGuage());
            _posiion = _ball.transform.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                hit.point = new Vector3(hit.point.x, 0f, hit.point.z);
                _posiion = new Vector3(_posiion.x, 0f, _posiion.z);

                _dis = _powerGauge * 50;
                _dir = new Vector3((hit.point.x - _ball.transform.position.x) * (-1),
                                    0f,
                                  (hit.point.z - _ball.transform.position.z) * (-1)).normalized;

                _ball.Shoot(_dis, _dir);
            }
        }

        if (Input.GetMouseButton(0))
        {
            DrawLine();
            _scrollbar.fillAmount += 0.001f;
            _powerGauge = _scrollbar.fillAmount;
            //_queueStick.transform.position = Vector3.Lerp(_queueStick.transform.position, (_ball.transform.position + (_dir * (-1))) * _powerGauge, 1f);
            _queueStickCenteralAxis.SetActive(true);
        }
    }

    IEnumerator ZeroPowerGuage()
    {
        while(_scrollbar.fillAmount > 0)
        {
            _scrollbar.fillAmount -= 0.01f;
            _powerGauge = _scrollbar.fillAmount;
            yield return null;
        }
        _queueStickCenteralAxis.SetActive(false);
        //_queueStick.transform.position = new Vector3(5f, 0f, 0f);
    }

    void DrawLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 ballPos = _ball.transform.position;
            ballPos.y = 0;

            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            Vector3 X = new Vector3(hitPoint.x, 0f, ballPos.z);

            float OX = Vector3.Distance(ballPos, X);
            float OP = Vector3.Distance(ballPos, hitPoint);

            float cosAngle = 0f;
            Vector3 tmp = (hitPoint - ballPos);

            if (tmp.x < 0 && tmp.z < 0)
            {
                cosAngle = Mathf.Acos(-OX / OP) * Mathf.Rad2Deg;
            }
            else if (tmp.x > 0 && tmp.z < 0)
            {
                cosAngle = Mathf.Acos(OX / OP) * Mathf.Rad2Deg;
            }
            else if (tmp.x < 0 && tmp.z > 0)
            {
                cosAngle = -(Mathf.Acos(-OX / OP) * Mathf.Rad2Deg);
            }
            else if (tmp.x > 0 && tmp.z > 0)
            {
                cosAngle = -(Mathf.Acos(OX / OP) * Mathf.Rad2Deg);
            }

            _queueStickCenteralAxis.transform.rotation = Quaternion.Slerp(_queueStickCenteralAxis.transform.rotation, 
                                                                          Quaternion.Euler(_queueStickCenteralAxis.transform.rotation.x, cosAngle, _queueStickCenteralAxis.transform.rotation.z), 
                                                                          100f);
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 ballPos = _ball.transform.position;
            ballPos.y = 0;

            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            Vector3 X = new Vector3(hitPoint.x, ballPos.y, ballPos.z);
            Vector3 Y = new Vector3(ballPos.x, ballPos.y, hitPoint.z);

            Gizmos.color = Color.white;
            Gizmos.DrawCube(X, new Vector3(0.2f, 0.2f, 0.2f));

            Gizmos.color = Color.green;
            Gizmos.DrawCube(Y, new Vector3(0.2f, 0.2f, 0.2f));

            Gizmos.color = Color.black;
            //Gizmos.DrawCube((hit.point - _ball.transform.position), new Vector3(0.2f, 0.2f, 0.2f));
            Gizmos.DrawCube(hitPoint, new Vector3(0.2f, 0.2f, 0.2f));


            Gizmos.color = Color.white;
            Gizmos.DrawLine(hitPoint, X); // PX

            Gizmos.color = Color.green;
            Gizmos.DrawLine(ballPos, hitPoint); // OP

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ballPos, X); // OX

            Gizmos.color = Color.red;
            Gizmos.DrawLine(hitPoint, Y); // PY
        }
    }
}
