using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private RadialMenu _radialMenu = null;

    private List<Item> _inventory = new List<Item>();

    //Test Code

    [SerializeField]
    private List<Item> _testItem = new List<Item>();

    [SerializeField]
    private Button _addItem = null;

    [SerializeField]
    private Button _removeItem = null;

    [SerializeField]
    private TMP_Text _itemCount = null;

    [SerializeField]
    private TMP_Text _notice = null;

    [SerializeField]
    private Image _selectItemImage = null;

    [SerializeField]
    private TMP_Text _selectItemName = null;

    private void Start()
    {
        _radialMenu.Initializ();

        _addItem.onClick.AddListener(AddItem);
        _removeItem.onClick.AddListener(RemoveItem);

        for(int i = 0; i < 4; i++)
        {
            _inventory.Add(_testItem[i]);
        }

        _itemCount.text = _inventory.Count.ToString();
    }

    private void AddItem()
    {
        if(_inventory.Count == 8)
        {
            _notice.text = "inventory is full. inventory count  : " + _inventory.Count;
            Invoke("RemoveNorice", 3f);
            return;
        }

        _inventory.Add(_testItem[_inventory.Count]);

        _itemCount.text = _inventory.Count.ToString();
    }

    private void RemoveItem()
    {
        if(_inventory.Count == 0)
        {
            _notice.text = "inventory count is zero. you can't remove item. inventory count  : " + _inventory.Count;
            Invoke("RemoveNorice", 3f);
            return;
        }

        _inventory.Remove(_inventory[_inventory.Count - 1]);

        _itemCount.text = _inventory.Count.ToString();
    }

    private void RemoveNorice()
    {
        _notice.text = string.Empty;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            _radialMenu.Open(_inventory);
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            _radialMenu.Close();
        }
    }

    public void SelectItem(Item item)
    {
        _selectItemImage.sprite = item.Image;
        _selectItemName.text = item.Name;
    }
}
