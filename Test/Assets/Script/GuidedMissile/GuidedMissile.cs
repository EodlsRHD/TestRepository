using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    private Transform Target = null;
    public float Speed = 30f;

    private Vector3 TargetDir = Vector3.zero;
    private Vector3 UpdateTargetDir = Vector3.zero;
    private Vector3 Dir = Vector3.zero;

    bool Set = false;

    public void SetMessile(Transform _target)
    {
        Target = _target;
    }

    private void OnEnable()
    {
        Set = true;
        StartCoroutine(MoveCo());
        Invoke("DestroyMissile", 10f);
    }

    private void Update()
    {
        if(Set == true)
        {
            UpdateTargetDir = (Target.transform.position - this.transform.position).normalized;
            if (TargetDir != UpdateTargetDir)
            {
                TargetDir = UpdateTargetDir;
                Dir = (UpdateTargetDir + TargetDir).normalized;
                Debug.DrawLine(transform.position, transform.position + Dir, Color.red);
            }
        }
    }

    private IEnumerator MoveCo()
    {
        while(this.gameObject.activeSelf == true)
        {
            transform.LookAt(this.transform.position + UpdateTargetDir);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Dir, Speed * Time.deltaTime);

            yield return null;
        }
    }

    public void DestroyMissile()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.forward);
    }

}
