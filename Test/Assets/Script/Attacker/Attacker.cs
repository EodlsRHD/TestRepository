using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackerHaviour
{
    idle,
    move,
    attack,
    die
}

public class Attacker : MonoBehaviour
{
    public Bullet bulletObj = null;

    private float cognitiveRange = 0f;

    private float attackRange = 0f;

    private float attackSpeed = 0f;

    [SerializeField]
    private float time = 0f;

    private AttackerHaviour _haviour;

    private Transform _target = null;

    private float _targetDis = 0f;


    private float lastAttackTime = 7f;

    private bool _isReload = false;

    void Start()
    {
        cognitiveRange = 30f;
        attackRange = 20f;
        attackSpeed = 7f;

        _haviour = AttackerHaviour.idle;
    }


    void Update()
    {
        time += Time.deltaTime;

        switch (_haviour)
        {
            case AttackerHaviour.idle:
                FindTarget();
                break;

            case AttackerHaviour.move:
                Move();
                break;

            case AttackerHaviour.attack:
                Attack();
                break;

            case AttackerHaviour.die:

                break;
        }

        if (_target == null)
        {
            _haviour = AttackerHaviour.idle;
            return;
        }

        Haviour();
    }

    void Haviour()
    {
        _targetDis = Vector3.Distance(_target.position, transform.position);

        if (_targetDis > cognitiveRange)
        {
            _haviour = AttackerHaviour.idle;
            _target = null;
        }

        if(_targetDis > attackRange)
        {
            _haviour = AttackerHaviour.move;
        }

        if (_targetDis < attackRange)
        {
            _haviour = AttackerHaviour.attack;
        }

        if(time >= lastAttackTime + attackSpeed)
        {
            Reload();
        }
    }

    void FindTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, cognitiveRange);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].CompareTag("Player"))
            {
                _target = colls[i].gameObject.transform;
                _haviour = AttackerHaviour.move;
                break;
            }
        }
    }

    void Move()
    {
        if (_targetDis < cognitiveRange && _targetDis > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime);
            transform.LookAt(_target);
        }
    }

    void Attack()
    {
        Vector3 target = new Vector3(_target.position.x, this.transform.position.y, _target.position.z);
        transform.LookAt(target);

        if(_isReload == false)
        {
            Bullet obj = Instantiate(bulletObj, this.transform.position, Quaternion.identity);
            obj.Set(10f, _target.position);
            
            lastAttackTime = time;
            _isReload = true;
        }
    }

    void Reload()
    {
        _isReload = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 30f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
