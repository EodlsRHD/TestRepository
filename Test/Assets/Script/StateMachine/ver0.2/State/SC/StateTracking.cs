using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateTracking : CharacterState
{
    private Vector3 _target = Vector3.zero;

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

        if (_character.distanceFromTarget <= _character.interactionDistance)
        {
            return false;
        }

        if (_character.healthPoint < _character.targetCharacter.healthPoint)
        {
            if (_character.isUnderAttack == true)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    public override void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        base.OnStateEnter(ref character, ref objectDetected);

        _character.NearestObject(ref objectDetected);

        Vector3 dir = (_character.targetCharacter.transform.position - _character.transform.position).normalized;
        _target = _character.transform.position + (dir * (_character.distanceFromTarget - _character.interactionDistance));
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

        float dis = Vector3.Distance(_target, _character.transform.position);

        if (dis <= 0.5f)
        {
            _character.navMeshAgent.SetDestination(_target);
            OnStateExit(_character);
        }
    }

    public override void OnStateExit(Character character)
    {
        character.TargetReset();
        _isActive = false;
        _target = Vector3.zero;
    }
}
