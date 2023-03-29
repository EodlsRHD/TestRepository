using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRunaway : CharacterState
{
    private Vector3 _target = Vector3.zero;

    private Vector3 _oneSecAfterPos = Vector3.zero;

    private float _time = 0f;

    public override bool Condition(ref List<Character> objectDetected, ref bool isActive)
    {
        if(objectDetected.Count == 0)
        {
            return false;
        }

        if(_character.targetCharacter == null)
        {
            return false;
        }

        if(_character.healthPoint >= _character.targetCharacter.healthPoint || _character.isUnderAttack == true)
        {
            return false;
        }

        return true;
    }

    public override void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        base.OnStateEnter(ref character, ref objectDetected);

        _character.NearestObject(ref objectDetected);

        Vector3 dir = (_character.transform.position - _character.targetCharacter.transform.position).normalized;
        _target = _character.transform.position + (dir * _character.searchRadius);
        _target.y = _character.transform.position.y;

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

        float dis = Vector3.Distance(_character.transform.position, _target);

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

        if (dis <= _character.transform.localScale.x * 0.5f)
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
