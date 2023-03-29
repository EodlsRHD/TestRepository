using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPost : MonoBehaviour , IComparable
{
    public string _title = string.Empty;

    public string _content = string.Empty;

    public int _index = -1;
    public void SetData(string title, string content, int index)
    {
        this.gameObject.name = index.ToString();
        _title = title;
        _content = content;
        _index = index;
    }

    public void ActiveObj(int i, int pagenum, Transform trans)
    {
        this.gameObject.transform.position += new Vector3(i * 3, -pagenum, 0);
        this.transform.SetParent(trans);

        this.gameObject.SetActive(true);
    }

    public void Die(Transform trans)
    {
        this.gameObject.SetActive(false);
        this.transform.position = new Vector3(0, 0, 0);
        this.transform.SetParent(trans);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public int CompareTo(object obj)
    {
        if(obj.GetType() != typeof(TestPost))
        {
            return -1;
        }
        TestPost tmp = (TestPost)obj;
        return this._index.CompareTo(tmp._index);
    }
}
