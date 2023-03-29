using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _power = 0f;

    private Vector3 _point = Vector3.zero;

    private float _dis = 0f;

    private float gravity = 9.8f;

    private float firingAngle = 45.0f;

    public void Set(float power, Vector3 point)
    {
        _power = power;
        _point = point;
        _dis = Vector3.Distance(_point, transform.position);
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        float projectile_Velocity = _dis / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        float flightDuration = _dis / Vx;
        transform.rotation = Quaternion.LookRotation(_point - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            Vector3 pos = new Vector3(0f, (Vy - (gravity * elapse_time)), Vx);
            transform.Translate(pos * Time.deltaTime);

            elapse_time += Time.deltaTime;
            yield return null;
        }

        if(elapse_time > flightDuration)
        {
            Invoke("Destroy", 1f);
            yield break;
        }
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }
}
