using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.AI.NavMeshAgent _navMeshAgent = null;

    [SerializeField]
    private CharacterInfo _info = null;

    [SerializeField]
    private float _healthPoint = 30f;

    public float _distanceFromTarget = 0f;

    [SerializeField]
    private bool _isUnderAttack = false;

    private int _layerMask = 0;

    private float _time = 0;

    private bool _isAction = false;

    private Vector3 _oneSecBeforePosition = Vector3.zero;
    
    [Header("Target Info")]
    [SerializeField]
    private Vector3 _targetPosition = Vector3.zero;

    [SerializeField]
    private Character _targetCharacter = null;

    [SerializeField]
    private List<Character> _objectDetected = new List<Character>();

    private Collider[] colls;

    public UnityEngine.AI.NavMeshAgent navMeshAgent
    {
        get { return _navMeshAgent; }
    }

    public float speed
    {
        get { return _info.speed; }
    }

    public float damage
    {
        get { return _info.damage; }
    }

    public float healthPoint
    {
        get { return _healthPoint; }
    }

    public float interactionDistance
    {
        get { return _info.interactionDistance; }
    }

    public float searchRadius
    {
        get { return _info.searchRadius; }
    }

    public float walkRadius
    {
        get { return _info.walkRadius; }
    }

    public float attackRate
    {
        get { return _info.attackRate; }
    }

    public float distanceFromTarget
    {
        get { return _distanceFromTarget; }
    }

    public bool isUnderAttack
    {
        get { return _isUnderAttack; }
        set { _isUnderAttack = value; }
    }

    public float time
    {
        get { return _time; }
        set { _time = value; }
    }

    public bool isAction
    {
        get { return _isAction; }
        set { _isAction = value; }
    }

    public Vector3 targetPosition
    {
        get { return _targetPosition; }
        set { _targetPosition = value; }
    }

    public Vector3 oneSecBeforePosition
    {
        get { return _oneSecBeforePosition; }
        set { _oneSecBeforePosition = value; }
    }

    public Character targetCharacter
    {
        get { return _targetCharacter; }
    }

    public void Initialize()
    {
        _healthPoint = _info.healthPoint;
        _distanceFromTarget = _info.searchRadius;
        _navMeshAgent.speed = _info.speed;
        _layerMask = 1 << LayerMask.NameToLayer("Unit");
        colls = new Collider[10];

        StateUIManager.instance.SetNum(this.gameObject.tag, true);
    }

    public void AroundSearch(bool use, string searchTag, System.Action<List<Character>> onCallbackObjectDetected)
    {
        if (use == false)
        {
            return;
        }

        if(searchTag == string.Empty || searchTag.Contains("Untagged"))
        {
            return;
        }

        _objectDetected.Clear();

        int collsNum = Physics.OverlapSphereNonAlloc(this.transform.position, _info.searchRadius, colls, _layerMask);

        for(int i = 0; i < collsNum; i++)
        {
            if(colls[i].gameObject.CompareTag(searchTag))
            {
                colls[i].gameObject.TryGetComponent(out Character character);
                _objectDetected.Add(character);
            }
        }

        onCallbackObjectDetected(_objectDetected);
    }

    public void NearestObject(ref List<Character> objectDetected)
    {
        Character targetCharacter = objectDetected[0];
        float dis = _info.searchRadius;

        for (int i = 0; i < objectDetected.Count; i++)
        {
            if (dis > Vector3.Distance(objectDetected[i].transform.position, this.transform.position))
            {
                dis = Vector3.Distance(objectDetected[i].transform.position, this.transform.position);
                targetCharacter = objectDetected[i];
            }
        }

        _targetCharacter = targetCharacter;
        _distanceFromTarget = Mathf.Round(Vector3.Distance(_targetCharacter.transform.position, this.transform.position) * 100f) * 0.01f;
    }

    public void Hit(Character Attacker, float damage) // 'Character' variable is TestCode
    {
        if(_healthPoint <= 0)
        {
            return;
        }

        _isUnderAttack = true;
        _healthPoint -= damage;

        if(_healthPoint <= 0)
        {
            StateUIManager.instance.SetNum(this.gameObject.tag, false);
            _isUnderAttack = false;
            this.gameObject.SetActive(false);
        }
    }

    public void TargetReset()
    {
        _targetCharacter = null;
        _distanceFromTarget = _info.searchRadius;
    }
}
