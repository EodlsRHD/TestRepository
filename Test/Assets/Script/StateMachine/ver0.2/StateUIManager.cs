using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class StateUIManager : MonoBehaviour
{
    private static StateUIManager _instance;
    public static StateUIManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new StateUIManager();
            }

            return _instance;
        }
    }

    [Header("Win or Lose")]
    [SerializeField]
    private TMP_Text _monstersText = null;

    [SerializeField]
    private TMP_Text _npcsText = null;

    [SerializeField]
    private TMP_Text _victoryText = null;

    [SerializeField]
    private Image _victoryBackground = null;

    [Header("Say Hello")]
    [SerializeField]
    private TMP_Text _toNameText = null;

    [SerializeField]
    private TMP_Text _contentText = null;

    [SerializeField]
    private Image _messageBackground = null;

    [Header("Info")]
    [SerializeField]
    private float _messageTime = 0f;

    private int _monsters = 0;

    private int _npcs = 0;

    public float messageTime
    {
        get { return _messageTime; }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void SetNum(string characterTag, bool isPlus)
    {
        if(isPlus == true)
        {
            if (characterTag.Contains("Monster"))
            {
                _monsters += 1;
                _monstersText.text = _monsters.ToString();
            }

            if (characterTag.Contains("Npc"))
            {
                _npcs += 1;
                _npcsText.text = _npcs.ToString();
            }
        }
        else if(isPlus == false)
        {
            if (characterTag.Contains("Monster"))
            {
                _monsters -= 1;
                _monstersText.text = _monsters.ToString();
            }

            if (characterTag.Contains("Npc"))
            {
                _npcs -= 1;
                _npcsText.text = _npcs.ToString();
            }

            if (_monsters == 0)
            {
                _victoryBackground.gameObject.SetActive(true);
                _victoryText.text = "VICTORY NPC";
            }

            if (_npcs == 0)
            {
                _victoryBackground.gameObject.SetActive(true);
                _victoryText.text = "VICTORY MONSTER";
            }
        }
    }

    public void SayHello(Character toCharacter, Character fromCharacter, float time)
    {
        if(_victoryBackground.gameObject.activeSelf == true)
        {
            return;
        }

        if(_messageBackground.gameObject.activeSelf == true)
        {
            if(time >= _messageTime)
            {
                _messageBackground.gameObject.SetActive(false);
                _toNameText.text = string.Empty;
                _contentText.text = string.Empty;
            }

            return;
        }

        _messageBackground.gameObject.SetActive(true);
        _toNameText.text = toCharacter.name;
        _contentText.text = "HI!   " + fromCharacter.name;
    }
}
