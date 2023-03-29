using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Me : MonoBehaviour
{
    [SerializeField]
    private GameObject camArm = null;

    private Vector3 dis;

    void Start()
    {
        
    }

    void Update()
    {
        float Ho = Input.GetAxis("Horizontal");
        float Ve = Input.GetAxis("Vertical");

        dis = new Vector3(Ho, 0f, Ve).normalized;
        transform.position += dis * Time.deltaTime * 17f;
        //CamPos();
    }

    void CamPos()
    {
        Vector2 mouseDelta = Input.mousePosition;
        Vector3 mouseRo = new Vector3(mouseDelta.y, mouseDelta.x, 0);

        if(mouseRo.x < 0)
        {
            
        }

        camArm.transform.rotation = Quaternion.Euler(mouseRo);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + dis);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(camArm.transform.position, camArm.transform.position + dis);
    }
}
