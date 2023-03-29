using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using pageIconControll;

public class PageIConController : MonoBehaviour
{
    [SerializeField]
    protected Icon _originalIconObj = null;

    [SerializeField]
    protected Transform _parent = null;

    [SerializeField]
    protected Transform _poolParent = null;

    protected List<Icon> allIconlist = new List<Icon>();

    protected List<Icon> poolIconlist = new List<Icon>();

    protected int _maxPageNum = 0;

    protected int _staypage = 1;

    [SerializeField]
    protected int viewIconMaxCount = 5;

    [SerializeField]
    protected int objPoolMaxCount = 20;

    protected Action<int> _onSelectPage = null;

    [SerializeField]
    protected Button _next = null;

    [SerializeField]
    protected Button _back = null;

    [Header("Test Property")]

    public TMP_Text _objPoolCount= null;

    public void Initializ(int maxPageNum, Action<int> onSelectPage)
    {
        _onSelectPage = onSelectPage;

        SetIcon(maxPageNum);
        IconControllButtonActive();

        _maxPageNum = maxPageNum;

        _next.onClick.AddListener(() => { _onSelectPage(_staypage + 1); });
        _back.onClick.AddListener(() => { _onSelectPage(_staypage - 1); });
    }

    public void SetIcon(int maxPageNum)
    {
        int tmp = 0;
        if(_maxPageNum <= maxPageNum)
        {
            tmp = maxPageNum - _maxPageNum;
            CreateIcon(tmp);
        }
        else if(_maxPageNum > maxPageNum)
        {
            tmp = _maxPageNum - maxPageNum;
            DeleteIcon(tmp);
        }

        for(int i = 0; i < allIconlist.Count; i++)
        {
            allIconlist[i].SetThisIconPageNum(i + 1);
        }

        _maxPageNum = maxPageNum;
    }

    private void CreateIcon(int useIcon)
    {
        if (useIcon > poolIconlist.Count)
        {
            int tmp = useIcon - poolIconlist.Count;
            for (int i = 0; i < tmp; i++)
            {
                Icon obj = Instantiate(_originalIconObj, _poolParent);
                obj.Initializ(_onSelectPage);
                poolIconlist.Add(obj);
            }
        }

        for (int i = useIcon - 1; i >= 0; i--)
        {
            allIconlist.Add(poolIconlist[i]);
            poolIconlist[i].gameObject.transform.SetParent(_parent);
            poolIconlist.Remove(poolIconlist[i]);
        }
    }

    private void DeleteIcon(int deleteIcon)
    {
        for (int i = deleteIcon - 1; i >= 0; i--)
        {
            allIconlist[i].transform.SetParent(_poolParent);
            allIconlist[i].gameObject.SetActive(false);
            poolIconlist.Add(allIconlist[i]);
            allIconlist.Remove(allIconlist[i]);
        }

        if (poolIconlist.Count > objPoolMaxCount)
        {
            int tmp = poolIconlist.Count - objPoolMaxCount;
            for (int i = tmp - 1; i >= 0; i--)
            {
                poolIconlist[i].Destroy();
                poolIconlist.Remove(poolIconlist[i]);
            }
        }

        _objPoolCount.text = poolIconlist.Count.ToString(); // Test
    }

    public void MoveIcon(int stayPage)
    {
        IconActive();
        _staypage = stayPage;

        int min = (_staypage - (viewIconMaxCount / 2)) - 1; 
        int max = (_staypage + (viewIconMaxCount / 2)) - 1;

        if(min > _maxPageNum - viewIconMaxCount)
        {
            min = _maxPageNum - viewIconMaxCount;
        }

        if (min < 0)
        {
            min = 0;
        }

        if (max > _maxPageNum - 1)
        {
            max = _maxPageNum - 1;
        }

        if (max < viewIconMaxCount - 1)
        {
            max = viewIconMaxCount - 1;
            if (_maxPageNum < viewIconMaxCount)
            {
                max = _maxPageNum - 1;
            }
        }

        for(int i = min; i < max + 1; i++)
        {
            allIconlist[i].gameObject.SetActive(true);
        }

        allIconlist[_staypage - 1].SetStayPage(_staypage);

        IconControllButtonActive();
    }

    private void IconControllButtonActive()
    {
        if (_staypage == 1)
        {
            _back.gameObject.SetActive(false);
        }
        else if (_staypage == _maxPageNum)
        {
            _next.gameObject.SetActive(false);
        }
        else
        {
            _next.gameObject.SetActive(true);
            _back.gameObject.SetActive(true);
        }
    }

    private void IconActive()
    {
        foreach (var one in allIconlist)
        {
            if (one.gameObject.activeSelf == true)
            {
                one.gameObject.SetActive(false);
            }
            one.SetStayPage(-1);
        }
    }
}
