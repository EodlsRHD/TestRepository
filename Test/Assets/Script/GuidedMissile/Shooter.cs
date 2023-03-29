using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform Target;
    public GuidedMissile Missile;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject tmp = Instantiate(Missile.gameObject, transform.position + Vector3.forward, transform.rotation);
            tmp.SetActive(true);
            tmp.GetComponent<GuidedMissile>().SetMessile(Target);
        }
    }

}
