using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerSet : MonoBehaviour
{
    enum eVerticesWall
    {
        NaN = -1,
        A,
        B,
        C,
        D
    }
    [SerializeField]
    private CameraControll _camController = null;

    [Header("Corner range")]
    [SerializeField]
    private float _width = 0f;

    [SerializeField]
    private float _length = 0f;

    [Header("Base point of rotation")]
    [SerializeField]
    private eVerticesWall _verticeWall = eVerticesWall.NaN;

    [Header("Angle of rotation")]
    [SerializeField]
    private float _lengthExitDegree = 0f;

    [SerializeField]
    private float _widthExitDegree = 0f;

    [Header("Escape and entry points")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _widthRnage = 0f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _lengthRnage = 0f;

    private Vector3 _a;
    private Vector3 _b;
    private Vector3 _c;
    private Vector3 _d;

    private Vector3 _wallVertice;
    private Vector3 _widthVertice;
    private Vector3 _lengthVertice;

    private Vector3 _widthPoint;
    private Vector3 _lengthPoint;

    private Vector3 _playerProjection;

    private float _exitDegree = 0f;

    private bool _isInside = false;
    private List<Vector3> _poslist = new List<Vector3>(); // TestCode

    private void Start()
    {
        Vector3 Center = this.transform.position;

        _a = new Vector3(Center.x - (_width / 2), Center.y, Center.z - (_length / 2));
        _b = new Vector3(Center.x + (_width / 2), Center.y, Center.z - (_length / 2));
        _c = new Vector3(Center.x + (_width / 2), Center.y, Center.z + (_length / 2));
        _d = new Vector3(Center.x - (_width / 2), Center.y, Center.z + (_length / 2));

        VerticeWall(_verticeWall);
    }

    void VerticeWall(eVerticesWall verticeWall)
    {
        switch (verticeWall)
        {
            case eVerticesWall.A:
                _wallVertice = _a;
                _widthVertice = _b;
                _lengthVertice = _d;
                break;

            case eVerticesWall.B:
                _wallVertice = _b;
                _widthVertice = _a;
                _lengthVertice = _c;
                break;

            case eVerticesWall.C:
                _wallVertice = _c;
                _widthVertice = _d;
                _lengthVertice = _b;
                break;

            case eVerticesWall.D:
                _wallVertice = _d;
                _widthVertice = _c;
                _lengthVertice = _a;
                break;
        }

        CornerEscapeEntryPoints(_wallVertice, _widthVertice, _lengthVertice);
    }

    void CornerEscapeEntryPoints(Vector3 wallVertice, Vector3 widthVertice, Vector3 lengthVertice)
    {
        Vector3 widthDir = (widthVertice - wallVertice).normalized;
        Vector3 lengthDir = (lengthVertice - wallVertice).normalized;

        float widthDis = Vector3.Distance(widthVertice, wallVertice) * _widthRnage;
        float lengthDis = Vector3.Distance(lengthVertice, wallVertice) * _lengthRnage;

        _widthPoint = wallVertice + (widthDir * widthDis);
        _lengthPoint = wallVertice + (lengthDir * lengthDis); 
    }

    private void Update()
    {
        Vector3 camArmPos = _camController.transform.position;
        camArmPos.y = this.transform.position.y;

        _poslist.Add(_camController.transform.position);

        if ((_a.x < camArmPos.x && _b.x > camArmPos.x && _a.z < camArmPos.z && _d.z > camArmPos.z) == false)
        {
            _isInside = false;
            return;
        }
        _isInside = true;

        Projection(camArmPos);
    }

    void Projection(Vector3 camArmPos)
    {
        _playerProjection = _widthPoint + Vector3.Project(camArmPos - _widthPoint, (_widthPoint - _lengthPoint).normalized);

        if(OverHalfVerticeSwitch(_verticeWall, _playerProjection, _wallVertice))
        {
            return;
        }

        float ProjectionLineDis = Vector3.Distance(_widthPoint, _lengthPoint);
        float PlayerProjectionDis = Vector3.Distance(_widthPoint, _playerProjection);
        float Proportion = (PlayerProjectionDis / ProjectionLineDis); 
        float Degree = Mathf.Abs(_lengthExitDegree - _widthExitDegree) * Proportion;

        if (_lengthExitDegree > _widthExitDegree)
        {
            _exitDegree = (_widthExitDegree + Degree);
        }
        else if (_lengthExitDegree < _widthExitDegree)
        {
            _exitDegree = (_widthExitDegree - Degree);
        }

        _camController.SetCornerCamera(_exitDegree);
    }

    bool OverHalfVerticeSwitch(eVerticesWall verticeWall, Vector3 playerProjection, Vector3 wallVertice)
    {
        switch(verticeWall)
        {
            case eVerticesWall.A:
                if (playerProjection.z < wallVertice.z)
                {
                    return true;
                }

                if(playerProjection.x < wallVertice.x)
                {
                    return true;
                }

                return false;

            case eVerticesWall.B:
                if (playerProjection.z < wallVertice.z)
                {
                    return true;
                }

                if (playerProjection.x > wallVertice.x)
                {
                    return true;
                }

                return false;

            case eVerticesWall.C:
                if (playerProjection.z > wallVertice.z)
                {
                    return true;
                }

                if (playerProjection.x > wallVertice.x)
                {
                    return true;
                }

                return false;

            case eVerticesWall.D:
                if (playerProjection.z > wallVertice.z)
                {
                    return true;
                }

                if (playerProjection.x < wallVertice.x)
                {
                    return true;
                }

                return false;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Vector3 Center = this.transform.position;
        Vector3 camArmPos = _camController.transform.position;
        camArmPos.y = Center.y;

        VerticeWall(_verticeWall);

        _a = new Vector3(Center.x - (_width / 2), Center.y, Center.z - (_length / 2));
        _b = new Vector3(Center.x + (_width / 2), Center.y, Center.z - (_length / 2));
        _c = new Vector3(Center.x + (_width / 2), Center.y, Center.z + (_length / 2));
        _d = new Vector3(Center.x - (_width / 2), Center.y, Center.z + (_length / 2));

        Gizmos.color = Color.black;
        Gizmos.DrawLine(_a, _b);
        Gizmos.DrawLine(_b, _c);
        Gizmos.DrawLine(_c, _d);
        Gizmos.DrawLine(_d, _a);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_a, 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_b, 0.3f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_c, 0.3f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_d, 0.3f);

        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(_widthPoint, 0.3f);
        Gizmos.DrawWireSphere(_lengthPoint, 0.3f);

        Vector3 dir = (_lengthPoint - _widthPoint).normalized;
        float dis = Vector3.Distance(_lengthPoint, _widthPoint);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(_widthPoint + (dir * (dis * 0.5f)), 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_widthPoint, _lengthPoint);

        if (_isInside == true)
        {
            Vector3 WidthDir = _widthVertice - _wallVertice;
            Vector3 WidthProjection = _widthVertice + Vector3.Project(camArmPos - _widthVertice, WidthDir.normalized);

            Vector3 lengthDir = _lengthVertice - _wallVertice;
            Vector3 lengthProjection = _lengthVertice + Vector3.Project(camArmPos - _lengthVertice, lengthDir.normalized);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(camArmPos, WidthProjection);
            Gizmos.DrawLine(camArmPos, lengthProjection);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(camArmPos, _playerProjection);

            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(_playerProjection, 0.3f);
        }

        Gizmos.color = Color.black;
        for (int i = 1; i < _poslist.Count; i++)
        {
            Gizmos.DrawLine(_poslist[i - 1], _poslist[i]);
        }
    }
}
