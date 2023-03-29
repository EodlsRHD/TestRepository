using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateInteraction : CharacterState
{
    private float _time = 0f;

    private float _endTime = 0f;

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

        if (_time != 0)
        {
            return false;
        }

        return true;
    }

    public override void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        base.OnStateEnter(ref character, ref objectDetected);

        if (_time != 0)
        {
            return;
        }

        _endTime = UnityEngine.Random.Range(1f, 3f);

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

        if (_time >= _endTime)
        {
            OnStateExit(_character);
        }
    }

    public override void OnStateExit(Character character)
    {
        _isActive = false;
        _time = 0f;
    }
}
