using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : CharacterState
{
    private Vector3 _target = Vector3.zero;

    private float _time = 0f;

    public override bool Condition(ref List<Character> objectDetected, ref bool isActive)
    {
        if (isActive == true)
        {
            return false;
        }

        if (objectDetected.Count == 0)
        {
            return false;
        }

        if (_character.distanceFromTarget > _character.interactionDistance)
        {
            return false;
        }

        if (_character.healthPoint < _character.targetCharacter.healthPoint)
        {
            if(_character.isUnderAttack == true)
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
        _target = _character.transform.position + (dir * _character.distanceFromTarget);

        _isActive = true;

        _onCallbackSetUpdate(OnStateStay);
    }

    public override void OnStateStay()
    {
        if (_isActive == false)
        {
            return;
        }

        _time += Time.deltaTime;

        if(_time >= _character.attackRate)
        {
            _character.targetCharacter.Hit(_character, _character.damage);

            OnStateExit(_character);
        }
    }

    public override void OnStateExit(Character character)
    {
        character.TargetReset();
        _time = 0f;
        _target = Vector3.zero;
        _isActive = false;
    }
}
