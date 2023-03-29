using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect = null;

    public RectTransform Rect
    {
        get
        {
            return _rect;
        }
    }

    private Vector2 _originV = new Vector2(0, 0);

    private Quaternion _originQ = Quaternion.Euler(0, 0, 0);

    private float _degree = 0f;

    public float Degree
    {
        get
        {
            return _degree;
        }
    }

    public void SetupPos(Vector2 v, Quaternion q, float degree)
    {
        _rect.anchoredPosition = v;
        _rect.rotation = q;
        _degree = degree;

        this.gameObject.SetActive(true);
    }

    public void RemovePos()
    {
        _rect.position = _originV;
        _rect.rotation = _originQ;

        this.gameObject.SetActive(false);
    }
}
