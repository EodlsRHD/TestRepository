using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BilliardsManager : MonoBehaviour
{
    private static BilliardsManager _instance;
    public static BilliardsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BilliardsManager();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    [SerializeField]
    private TMP_Text _scoreText = null;

    private int _scoreCount = 0;

    private Vector3 _redBallSpawnPos = Vector3.zero;

    //[SerializeField]
    //private BallSpawner _ballSpawner = null;

    //[Header("GameSetting")]
    //public float FrictionalForce = 0f;


    void Awake()
    {
        _instance = this;
        _redBallSpawnPos = new Vector3(6.5f, 0.25f, 0f);
    }

    public void ScoreCount(string colorType, Ball ball)
    {
        if(colorType.Contains("red"))
        {
            _scoreCount -= 1;
            ball.gameObject.transform.position = _redBallSpawnPos;
        }
        else
        {
            _scoreCount += 1;
            ball.gameObject.SetActive(false);
        }
        _scoreText.text = _scoreCount.ToString();
    }
}
