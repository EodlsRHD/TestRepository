using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Example : MonoBehaviour
{
    PageController<TestPost> _pageController = new PageController<TestPost>();

    Func<TestPost, TestPost, int> _sort = (a, b) => { return a._index > b._index ? 1 : -1; };

    List<TestPost> afterPostlist = new List<TestPost>();

    ePageSortType sorttype = ePageSortType.Ascending;

    public PageIConController _listcontroll = null;

    ///////////////////////////////////////////////////////////////////////////////

    [Header("Test Property")]

    public TestPost _testPost = null;

    List<TestPost> testlist = new List<TestPost>();

    public int testNum = 100;

    public int _lastPage = 0;

    public int _maxPostCount = 10;

    public int _stayPage = 0;

    public TMP_Text _lastPageText = null;

    public TMP_Text _currentPage = null;

    public TMP_Text _objCount = null;

    public TMP_Text _maxpostNum = null;

    public TMP_Text _sortTypeText = null;

    public TMP_InputField selectPage = null;

    public TMP_InputField _postNumChange = null;

    public TMP_InputField _maxPostNumChange = null;

    public Image _backGround = null;

    public Button _testStartButton = null;

    public Button _sortTypeTextButton = null;

    public Button __maxPostNumChangeButton = null;

    public Button _postNumChangeButton = null;

    public Button _changePageButton = null;

    public Button _nextPageButton = null;

    public Button _backPageButton = null;

    public Button _firstPageButton = null;

    public Button _lastPageButton = null;

    public Transform _parent = null;

    public Transform _activeParent = null;

    private void Start()
    {
        _pageController.Initializ(_sort, SortObj, _listcontroll.SetIcon, _listcontroll.MoveIcon);

        _changePageButton.onClick.AddListener(SelectPage);
        _nextPageButton.onClick.AddListener(Next);
        _backPageButton.onClick.AddListener(Back);
        _firstPageButton.onClick.AddListener(First);
        _lastPageButton.onClick.AddListener(Last);
        _postNumChangeButton.onClick.AddListener(ChangeObject);
        __maxPostNumChangeButton.onClick.AddListener(ChangeMaxPost);
        _sortTypeTextButton.onClick.AddListener(ChangeSort);
        _testStartButton.onClick.AddListener(TestStart);

        _objCount.text = testNum.ToString(); // Test
        _maxpostNum.text = _maxPostCount.ToString(); // Test
        _sortTypeText.text = sorttype.ToString(); // Test
    }

    public void TestStart()
    {
        _backGround.gameObject.SetActive(false);
        for (int i = 0; i < testNum; i++)
        {
            TestPost tmp = Instantiate(_testPost, _parent);
            tmp.SetData(i.ToString(), i.ToString(), i);
            testlist.Add(tmp);
            tmp.gameObject.SetActive(false);
        }

        _lastPage = testNum / _maxPostCount;
        if ((_lastPage % _maxPostCount) > 0)
        {
            _lastPage += 1;
        }

        _listcontroll.Initializ(_lastPage, SelectPage);
        _pageController.OnSet(_maxPostCount, testlist, sorttype);
    }


    private void ChangeObject()
    {
        First(); // Test

        int Num = int.Parse(_postNumChange.text);

        if(testNum == Num)
        {
            return;
        }

        testNum = Num;
        foreach (var one in testlist)
        {
            one.Destroy();
        }
        testlist.Clear();


        for (int i = 0; i < testNum; i++)
        {
            TestPost tmp = Instantiate(_testPost, _parent);
            tmp.SetData(i.ToString(), i.ToString(), i);
            testlist.Add(tmp);
            tmp.gameObject.SetActive(false);
        }

        _objCount.text = testNum.ToString(); // Test
        _maxpostNum.text = _maxPostCount.ToString(); // Test
        _pageController.OnSet(_maxPostCount, testlist, sorttype);
    }

    private void ChangeSort()
    {
        if (sorttype == ePageSortType.Ascending)
        {
            sorttype = ePageSortType.Descending;
        }
        else if(sorttype != ePageSortType.Ascending)
        {
            sorttype = ePageSortType.Ascending;
        }
        _sortTypeText.text = sorttype.ToString(); // Test
        _pageController.OnChangeSort(sorttype);
    }

    private void ChangeMaxPost()
    {
        //First();

        int num = int.Parse(_maxPostNumChange.text); // Input
        _pageController.OnChangeMaxPost(num);
        _maxPostCount = num;

        _maxpostNum.text = _maxPostCount.ToString(); // Test
    }

    private void SortObj(List<TestPost> result, int lastPage, int stayPage)
    {
        if(result.Count == 0)
        {
            return;
        }

        _stayPage = stayPage;

        foreach (var one in afterPostlist)
        {
            one.Die(_parent);
        }
        afterPostlist.Clear();

        for (int i = 0; i < result.Count; i++)
        {
            result[i].ActiveObj(i, stayPage, _activeParent);
            afterPostlist.Add(result[i]);
        }

        _lastPage = lastPage;
        _lastPageText.text = lastPage.ToString(); // Test
        _currentPage.text = stayPage.ToString(); // Test
    }

    public void Next()
    {
        _pageController.OnEvent(ePageEvent.next);
    }

    public void Back()
    {
        _pageController.OnEvent(ePageEvent.back);
    }

    public void First()
    {
        _pageController.OnEvent(ePageEvent.first);
    }

    public void Last()
    {
        _pageController.OnEvent(ePageEvent.last);
    }

    private void SelectPage()
    {
        int page = int.Parse(selectPage.text);
        _pageController.OnEvent(ePageEvent.move, page);
    }

    private void SelectPage(int num)
    {
        _pageController.OnEvent(ePageEvent.move, num);
    }
}
