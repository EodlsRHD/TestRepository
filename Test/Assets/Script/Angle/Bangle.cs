using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bangle : MonoBehaviour
{
    [SerializeField]
    private GameObject _x = null;

    [SerializeField]
    private GameObject _y = null;

    [SerializeField]
    private GameObject _z = null;

    Vector3 O = Vector3.zero;
    Vector3 P = Vector3.zero;
    Vector3 Q = Vector3.zero;

    float OQ = 0f;
    float OP = 0f;

    float cosAngle = 1f;

    enum Location
    {
        x,
        y,
        z
    }

    void Update()
    {
        BangleSphere(Location.x, _x);
        //BangleSphere(Location.y, _y);
        //BangleSphere(Location.z, _z);
    }

    void BangleSphere(Location location, GameObject _obj)
    {
        O = this.transform.position;
        float rad = Mathf.Cos(cosAngle);

        switch (location)
        {
            case Location.x:

                break;

            case Location.y:

                break;

            case Location.z:

                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, _z.transform.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, _x.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, _y.transform.position);
    }
}
