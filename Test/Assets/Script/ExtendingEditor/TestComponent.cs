using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public Vector3 startPoint = new Vector3(0f, 0f, 0f);
    public Vector3 endPoint = new Vector3(5f, 5f, 5f);

    public float moveSpeed = 1f;
    public bool TestStart = false;

    public Material mat;

    private Vector3 _targetPoint = Vector3.zero;
    private Vector3 _thisGameObjectPosition = Vector3.zero;

    public List<Vector3> _pointList = new List<Vector3>();

    void Start()
    {
        _targetPoint = endPoint;
        TestStart = false;
    }

    void Update()
    {
        _thisGameObjectPosition = this.gameObject.transform.position;
        if (TestStart == true)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(_thisGameObjectPosition, _targetPoint, Time.deltaTime * moveSpeed);

            float Dis = Vector3.Distance(_targetPoint, _thisGameObjectPosition);
            
            if(Dis < 0.5f)
            {
                _pointList.Add(endPoint);
                endPoint = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                _targetPoint = endPoint;
            }

            if(_pointList.Count > 0)
            {
                startPoint = _pointList[_pointList.Count - 1];
            }

            if(_pointList.Count > 30)
            {
                _pointList.Remove(_pointList[0]);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(10f, 10f, 10f));

        for (int i = 1; i < _pointList.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_pointList[i - 1], _pointList[i]);
        }

        if (_pointList.Count > 0)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(_pointList[_pointList.Count - 1], transform.position);
        }
    }
}
