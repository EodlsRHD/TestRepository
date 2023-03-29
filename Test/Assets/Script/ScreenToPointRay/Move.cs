using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private GameObject _obj = null;
    private void Update()
    {
        this.transform.position += new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime, 0f, Input.GetAxis("Vertical") * Time.deltaTime);
        //_obj.transform.position = new Vector3(this.transform.position.x, _obj.transform.position.y, this.transform.position.z);

        RaycastHit hit;
        
        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10f))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}
