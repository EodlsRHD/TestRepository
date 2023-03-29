using System.Collections;
using System.Collections.Generic;
using System;

public enum ePageSortType
{
    Ascending,
    Descending
}

public enum ePageEvent
{
    NON = -1,
    first,
    last,
    next,
    back,
    move
}

public class PageController<T> where T : IComparable
{
    int _maxPostCount = -1;

    int _stayPageNum = 1;

    int _maxPageNum = 0;

    int _remainderPostNum = 0;

    ePageSortType _sorttype;

    Action<List<T>, int, int> _onResultCallback = null;

    List<T> _postList = new List<T>();

    List<T> retultlist = new List<T>();

    Func<T, T, int> _sort = null;

    Action<int> _onSetIcon = null;

    Action<int> _onMoveIcon = null;

    public void Initializ(Func<T, T, int> sort, Action<List<T>, int, int> onResultCallback, Action<int> onSetIcon, Action<int> onMoveIcon)
    {
        _onResultCallback = onResultCallback;
        _sort = sort;
        _onSetIcon = onSetIcon;
        _onMoveIcon = onMoveIcon;
    }

    public void OnSet(int maxPostCount, List<T> postList, ePageSortType sortType = ePageSortType.Ascending)
    {
        _maxPostCount = maxPostCount;
        _postList = postList;
        _sorttype = sortType;

        _maxPageNum = (postList.Count / _maxPostCount);
        _remainderPostNum = _postList.Count % _maxPostCount;
        if(_remainderPostNum > 0)
        {
            _maxPageNum += 1;
        }
        _onSetIcon(_maxPageNum);
        Sort(_sorttype);
    }

    public void OnEvent(ePageEvent pageEvent = ePageEvent.NON, int pageNum = 0)
    {
        int i = 1;
        switch (pageEvent)
        {
            case ePageEvent.first:
                break;

            case ePageEvent.last:
                i = _maxPageNum;
                break;

            case ePageEvent.next:
                if ((_stayPageNum + 1) > _maxPageNum)
                {
                    return;
                }
                i = _stayPageNum + 1;
                break;

            case ePageEvent.back:
                if ((_stayPageNum - 1) < 1)
                {
                    return;
                }
                i = _stayPageNum - 1;
                break;
            case ePageEvent.move:
                if (pageNum > _maxPageNum)
                {
                    return;
                }
                i = pageNum;
                break;
        }
        SetPage(i);
    }

    public void OnChangeSort(ePageSortType sortType)
    {
        _sorttype = sortType;

        Sort(_sorttype);
    }

    public void OnChangeMaxPost(int num)
    {
        _maxPostCount = num;
        _maxPageNum = _postList.Count / _maxPostCount;
        _remainderPostNum = _postList.Count % _maxPostCount;
        if (_remainderPostNum > 0)
        {
            _maxPageNum += 1;
        }
        _onSetIcon(_maxPageNum);
        Sort(_sorttype);
    }

    private void Sort(ePageSortType sortType)
    {
        switch (sortType)
        {
            case ePageSortType.Ascending:
                //_postList.Sort((a, b) => { return _sort.Invoke(a, b); });
                _postList.Sort((a, b) => { return a.CompareTo(b); });
                break;

            case ePageSortType.Descending:
                //_postList.Sort((a, b) => { return -_sort.Invoke(a, b); });
                _postList.Sort((a, b) => { return b.CompareTo(a); });
                break;
        }

        SetPage(_stayPageNum);
    }

    private void SetPage(int pageNum)
    {
        int min = ((pageNum * _maxPostCount) - _maxPostCount);
        int max = (pageNum * _maxPostCount);

        for (int i = min; i < max; i++)
        {
            retultlist.Add(_postList[i]);
            if (i == _postList.Count - 1)
            {
                break;
            }
        }

        _stayPageNum = pageNum;

        _onMoveIcon(_stayPageNum);
        Result(retultlist);
    }

    private void Result(List<T> result)
    {
        _onResultCallback(result, _maxPageNum, _stayPageNum);
        retultlist.Clear();
    }
}
