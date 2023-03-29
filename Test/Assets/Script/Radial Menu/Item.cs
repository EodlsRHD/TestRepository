using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{

    [SerializeField]
    private RectTransform _rect = null;

    private Image _image;

    private TMP_Text _name;

    public Sprite Image
    {
        get
        {
            return _image.sprite;
        }
    }

    public string Name
    {
        get
        {
            return _name.text;
        }
    }

    public void Initializ(Vector2 pos, Sprite sprite, string name)
    {
        _rect.anchoredPosition = pos;
        _image.sprite = sprite;
        _name.text = name;

        this.gameObject.SetActive(true);
    }

    public void TestActiveFalse()
    {
        _image.sprite = null;
        _name.text = string.Empty;

        this.gameObject.SetActive(false);
    }
}
