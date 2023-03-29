using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    private Vector3 movePos;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                movePos = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
            }
        }

        this.transform.position = Vector3.MoveTowards(this.transform.position, movePos, Time.deltaTime * 10f);
    }

}
