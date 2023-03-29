using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHello : CharacterState
{
    private Character _sayHelloCharacter = null;

    private float _time = 0f;

    public override bool Condition(ref List<Character> objectDetected, ref bool isActive)
    {
        if(isActive == true)
        {
            return false;
        }

        if(objectDetected.Count == 0)
        {
            return false;
        }

        if (_character.distanceFromTarget > _character.searchRadius)
        {
            return false;
        }

        return true;
    }

    public override void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        base.OnStateEnter(ref character, ref objectDetected);

        if (objectDetected.Count > 0)
        {
            _character.NearestObject(ref objectDetected);

            _isActive = true;
        }

        _onCallbackSetUpdate(OnStateStay);
    }

    public override void OnStateStay()
    {
        if (_isActive == false)
        {
            return;
        }

        if(_character.targetCharacter == _sayHelloCharacter)
        {
            OnStateExit(_character);
            return;
        }

        _time += Time.deltaTime;
        StateUIManager.instance.SayHello(_character, _character.targetCharacter, _time);

        if (_time >= StateUIManager.instance.messageTime)
        {
            _sayHelloCharacter = _character.targetCharacter;
            OnStateExit(_character);
        }
    }

    public override void OnStateExit(Character character)
    {
        character.TargetReset();
        _time = 0f;
        _isActive = false;
    }
}
