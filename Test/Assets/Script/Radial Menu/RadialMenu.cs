using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField]
    private Manager _radialmenuManager = null;

    [SerializeField]
    private RectTransform _center = null;

    [SerializeField]
    private RectTransform _itemSlotPool = null;

    [SerializeField]
    private RectTransform _iinePool = null;

    [SerializeField]
    private GameObject _background = null;

    [SerializeField]
    private Line _originalLine = null;

    [SerializeField]
    private Item _origilasItem = null;

    [SerializeField]
    private float _radius = 100;

    private float quotient = 0f;

    private List<Line> _lineList = new List<Line>(8);

    private List<Item> _itemList = new List<Item>(8);

    private List<Line> _activeLineList = new List<Line>();

    private List<Item> _activeItemList = new List<Item>();

    private bool _isRadialMenuOpen = false;

    public void Initializ()
    {
        for (int i = 0; i < 8; i++)
        {
            Line tmp = Instantiate(_originalLine, _iinePool);
            tmp.name = tmp.name + i;
            _lineList.Add(tmp);
            tmp.gameObject.SetActive(false);
        }

        for(int i = 0; i < 8; i++)
        {
            Item tmp = Instantiate(_origilasItem, _itemSlotPool);
            tmp.name = tmp.name + i;
            _itemList.Add(tmp);
            tmp.gameObject.SetActive(false);
        }
    }

    public void Open(List<Item> inventory)
    {
        _background.SetActive(true);

        if(inventory.Count == 0)
        {
            return;
        }

        if (inventory.Count == 1)
        {
            _itemList[0].Initializ(Vector2.up * _radius, inventory[0].Image, inventory[0].Name);
            return;
        }

        quotient = 360 / inventory.Count;
        float remain = (360 % inventory.Count);

        float Degree = 0f;
        float r = 0f;
        float x = 0f;
        float y = 0f;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            Degree += quotient;
            r += remain;
            if (r >= 10)
            {
                Degree += 1f;
                r -= 10f;
            }

            x = _center.anchoredPosition.x - (_radius * Mathf.Sin(Mathf.Deg2Rad * Degree));
            y = _center.anchoredPosition.y + (_radius * Mathf.Cos(Mathf.Deg2Rad * Degree));

            _lineList[i].SetupPos(new Vector2(x, y), Quaternion.Euler(0, 0, Degree), Degree);
            _activeLineList.Add(_lineList[i]);
        }

        Vector3 one = Vector3.zero;
        Vector3 two = Vector3.zero;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (i == inventory.Count - 1)
            {
                one = (_lineList[inventory.Count - 1].Rect.anchoredPosition - _center.anchoredPosition);
                two = (_lineList[0].Rect.anchoredPosition - _center.anchoredPosition);
            }
            else if (i != inventory.Count - 1)
            {
                one = (_lineList[i].Rect.anchoredPosition - _center.anchoredPosition);
                two = (_lineList[i + 1].Rect.anchoredPosition - _center.anchoredPosition);
            }

            Vector3 plus = (one + two).normalized;
            if (inventory.Count == 2 && i == 0)
            {
                plus = (-1) * plus;
            }
            _itemList[i].Initializ(plus * _radius, inventory[i].Image, inventory[i].Name);
            _activeItemList.Add(_itemList[i]);
        }

        _isRadialMenuOpen = true;
    }

    private void Update()
    {
        if(_isRadialMenuOpen == false)
        {
            return;
        }

        Vector2 mousePos = Input.mousePosition;

        if(Input.GetMouseButtonUp(0))
        {
            SelectItem(mousePos);
        }
    }
    
    void SelectItem(Vector2 mousePos)
    {
        float newX = mousePos.x - (Screen.width * 0.5f);
        float newY = mousePos.y - (Screen.height * 0.5f);
        Vector2 newMouseDelta = new Vector2(newX, newY);

        float OP = Vector2.Distance(_center.anchoredPosition, newMouseDelta);
        float OQ = Vector2.Distance(_center.anchoredPosition, new Vector2(_center.anchoredPosition.x, newMouseDelta.y));
        Vector2 tmp = (newMouseDelta - _center.anchoredPosition).normalized;

        float resultDegree = 0f;
        if(tmp.x > 0 && tmp.y > 0)
        {
            resultDegree = 180 + (Mathf.Acos(-OQ / OP) * Mathf.Rad2Deg);
        }
        else if(tmp.x < 0 && tmp.y > 0)
        {
            resultDegree = (Mathf.Acos(OQ / OP) * Mathf.Rad2Deg);
        }
        else if(tmp.x < 0 && tmp.y < 0)
        {
            resultDegree = (Mathf.Acos(-OQ / OP) * Mathf.Rad2Deg);
        }
        else if(tmp.x > 0 && tmp.y < 0)
        {
            resultDegree = 180 + (Mathf.Acos(OQ / OP) * Mathf.Rad2Deg);
        }

        for(int i = 0; i < _activeLineList.Count; i++)
        {
            if(i == _activeLineList.Count - 1)
            {
                if (0 < resultDegree && _activeLineList[i].Degree > resultDegree)
                {
                    _radialmenuManager.SelectItem(_activeItemList[0]);
                    break;
                }
            }

            if(_activeLineList[i].Degree < resultDegree && _activeLineList[i + 1].Degree > resultDegree)
            {
                _radialmenuManager.SelectItem(_activeItemList[i + 1]);
                break;
            }
        }
    }

    public void Close()
    {
        for(int i = 0; i < _itemList.Count; i++)
        {
            _lineList[i].RemovePos();
            _itemList[i].TestActiveFalse();
        }

        _activeLineList.Clear();
        _activeItemList.Clear();

        _isRadialMenuOpen = false;
        _background.SetActive(false);
    }
}