using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeMenu : MonoBehaviour
{
    [Header("Button")]
    [SerializeField]
    private Button _type1 = null;

    [SerializeField]
    private Button _type2 = null;

    [SerializeField]
    private Button _type3 = null;

    [Header("Content")]
    [SerializeField]
    private List<GameObject> _contents = new List<GameObject>();


    private void Start()
    {
        _type1.onClick.AddListener(() => { ActiveContent(_contents[0]); });
        _type2.onClick.AddListener(() => { ActiveContent(_contents[1]); });
        _type3.onClick.AddListener(() => { ActiveContent(_contents[2]); });
    }

    private void ActiveContent(GameObject content)
    {
        foreach (var item in _contents)
        {
            item.SetActive(false);
        }

        content.SetActive(true);
    }
}