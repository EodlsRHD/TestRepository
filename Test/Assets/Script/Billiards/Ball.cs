using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private float _dis = 0f;

    private Vector3 _dir = Vector3.zero;

    private float _radius = 0.25f;

    private bool _isMove = false;

    private int _layerMask = 0;

    private Vector3 _prevPos = Vector3.zero;

    private Vector3 _toPos = Vector3.zero;

    [SerializeField]
    private string _colorType = string.Empty;

    public float Dis
    {
        set
        {
            _dis = value;
        }
    }

    public Vector3 Dir
    {
        set
        {
            _dir = value;
        }
    }

    public bool IsMove
    {
        get
        {
            return _isMove;
        }
    }

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("BALL");
    }

    public void Shoot(float dis, Vector3 dir)
    {
        _dis = dis;
        _dir = dir.normalized;
    }

    public void Hit(float dis, Vector3 dir)
    {
        _dis = (dis * 0.9f);
        if (dis < _radius)
        {
            _dis = 0.25f;
        }
        _dir = new Vector3(dir.x * (-1), 0f, dir.z * (-1));

        Debug.DrawLine(transform.position, _dir, Color.black);
    }

    private void Update()
    {
        if (_dis < 0)
        {
            _isMove = false;
            return;
        }


        _prevPos = this.transform.position;
        transform.Translate(_dir * _dis * Time.deltaTime);
        _toPos = this.transform.position;

        RAY(_prevPos, _toPos);

        _dis -= 0.03f;
        _isMove = true;
    }

    void RAY(Vector3 prevPos, Vector3 toPos)
    {
        RaycastHit hit;
        
        if (Physics.SphereCast(prevPos - (_dir * _radius), _radius, _dir, out hit, Mathf.Abs((prevPos - (toPos + (_dir * _radius))).magnitude)))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                return;
            }

            if (hit.collider.CompareTag("bar"))
            {
                _dir = Vector3.Reflect(_dir, hit.normal).normalized;
                _dir.y = 0f;
                _dis -= (_dis * 0.3f);
            }

            if (hit.collider.CompareTag("ball"))
            {
                var ball = hit.collider.gameObject.GetComponent<Ball>();

                Vector3 angleOfReflection = Vector3.Reflect(_dir, hit.normal).normalized;
                angleOfReflection.y = 0;
                //float dot = Vector3.Dot(angleOfReflection, hit.normal);
                //float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                _dir = angleOfReflection;

                ball.Hit(_dis, angleOfReflection);
                _dis -= (_dis * 0.6f);
            }

            if (hit.collider.CompareTag("hole"))
            {
                BilliardsManager.Instance.ScoreCount(_colorType, this);
                _dis = 0;
            }
        }
    }
}