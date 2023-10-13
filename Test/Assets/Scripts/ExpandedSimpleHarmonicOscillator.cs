using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandedSimpleHarmonicOscillator : MonoBehaviour
{
    public Transform pivot;
    public float speed = 2.0f;
    public GameObject plane;

    float gravity = 1.0f;

    float startAngle, endAngle;
    Vector3 startPlaneNormal;

    Vector3 startPO;
    float distance;

    public GameObject lineRenderer;
    LineRenderer[] lrs;

    float startRot = 0.0f;

    bool isDragging;
    bool isJump;

    [Range(0.0f, 45.0f)]
    public float theta = 30.0f;
    [Range(0.0f, 1.0f)]
    public float ratio = 0.1f;

    float totalEnergy;
    void setLineRenderer(LineRenderer lr, Color color)
    {
        lr.startWidth = lr.endWidth = .1f;
        lr.material.color = color;

        lr.positionCount = 2;
    }

    Vector3 getNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Plane p = new Plane(a, b, c);
        return p.normal;
    }

    void setPlane(Vector3 a, Vector3 b, Vector3 c)
    {
        plane.transform.position = a;
        plane.transform.up = getNormal(a, b, c);
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseDrag()
    {
        float distance = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);

        objPos.x = 0;
        this.transform.position = objPos;
    }

    void OnMouseUp()
    {
        initSettings();
        isDragging = false;
    }

    void initSettings()
    {
        float angle = Vector3.Angle(this.transform.position - pivot.position, Vector3.down);

        startAngle = 0.0f; endAngle = angle * 2;

        Vector3 normal
            = getNormal(pivot.position + Vector3.down, pivot.transform.position, this.transform.position);

        startPlaneNormal = normal;

        setPlane(pivot.position + Vector3.down, pivot.transform.position, this.transform.position);

        startPO = (this.transform.position - pivot.position).normalized;
        distance = Vector3.Distance(this.transform.position, pivot.position);

        startRot = 0.0f;

        totalEnergy = this.transform.position.y; // Potential Energy = mgh
    }

    void Start()
    {
        initSettings();

        lrs = lineRenderer.GetComponentsInChildren<LineRenderer>();
        setLineRenderer(lrs[0], Color.blue);
        lrs[0].SetPosition(0, pivot.position);

        setLineRenderer(lrs[1], Color.red);     
    }

    int getCurrentState()
    {
        float hPi = Mathf.PI / 2.0f;
        return (int)(startRot / hPi) % 4;
    }

    float getNormalDirection()
    {
        return getCurrentState() < 2 ? 1.0f : -1.0f;
    }

    void Update()
    {
        lrs[0].SetPosition(1, this.transform.position);    
    }

    void makeArrow(LineRenderer lr, Vector3 start, Vector3 end, Vector3 normal)
    {
        Vector3 vES = (start - end) * ratio;
        Vector3 dot1 = end + Quaternion.AngleAxis(theta, normal) * vES;
        Vector3 dot2 = end + Quaternion.AngleAxis(360 - theta, normal) * vES;

        lr.positionCount = 5;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.SetPosition(2, dot1);
        lr.SetPosition(3, dot2);
        lr.SetPosition(4, end);
    }

    void FixedUpdate()
    {
        if (isDragging || isJump) return;

        startRot += (Time.fixedDeltaTime * speed);

        float angle 
            = Mathf.LerpAngle(startAngle, endAngle, (Mathf.Sin(startRot - Mathf.PI / 2) + 1.0f) / 2.0f);

        Vector3 vPO 
            = Quaternion.AngleAxis(angle, startPlaneNormal) * startPO;

        this.transform.position 
            = pivot.transform.position + (vPO).normalized * distance;

        float kinetic = gravity * (totalEnergy - this.transform.position.y);
        float v = Mathf.Sqrt(2 * kinetic); // k = 1/2 m v^2 -> v = sqrt(2k/m)
        Vector3 verocity 
            = (Quaternion.AngleAxis(90, startPlaneNormal) * vPO).normalized * getNormalDirection();

        makeArrow(lrs[1], this.transform.position, this.transform.position + verocity * v, startPlaneNormal);
    }
}