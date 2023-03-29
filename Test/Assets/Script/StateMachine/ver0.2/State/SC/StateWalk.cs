using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class StateWalk : CharacterState
{
    private Vector3 _target = Vector3.zero;

    private Vector3 _oneSecAfterPos = Vector3.zero;

    private float _time = 0f;

    public override bool Condition(ref List<Character> objectDetected, ref bool isActive)
    {
        if (isActive == true)
        {
            return false;
        }

        if (objectDetected.Count > 0)
        {
            return false;
        }

        return true;
    }

    public override void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        base.OnStateEnter(ref character, ref objectDetected);

        if (_target != Vector3.zero)
        {
            return;
        }

        Vector3 randomPos = _character.transform.position + Random.insideUnitSphere * _character.walkRadius;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, _character.searchRadius, NavMesh.AllAreas))
        {
            _target = hit.position;
        }
        else
        {
            _target = _character.transform.position;
        }

        _character.navMeshAgent.SetDestination(_target);

        _isActive = true;

        _onCallbackSetUpdate(OnStateStay);
    }

    public override void OnStateStay()
    {
        if (_isActive == false)
        {
            return;
        }

        float dis = Vector3.Distance(_target, _character.transform.position);

        if(_time == 0f)
        {
            _oneSecAfterPos = _character.transform.position;
        }

        _time += Time.deltaTime;

        if(_time >= 1f)
        {
            _time = 0f;

            if(Vector3.Distance(_oneSecAfterPos, _character.transform.position) < 1f)
            {
                OnStateExit(_character);
                return;
            }
        }

        if(dis <= _character.transform.localScale.y)
        {
            OnStateExit(_character);
        }
    }

    public override void OnStateExit(Character character)
    {
        _isActive = false;
        _target = Vector3.zero;
    }
}
