using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    void Update()
    {
        RaycastHit hit;
        if(Physics.BoxCast(transform.position, new Vector3(0.5f, 0.5f, 0.5f), transform.position, out hit))
        {
            if(hit.collider.CompareTag("ball"))
            {
                Ball ball = hit.collider.gameObject.GetComponent<Ball>();
            }
        }
    }
}
