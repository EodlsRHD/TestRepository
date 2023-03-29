using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : OldState
{
    private float _time = 0f;

    private float _endTiem = 0f;

    private float _stateCount = 0f;

    public override bool Condition(List<Npc> targetPos, List<Npc> evasionPos, bool isActive)
    {
        if (isActive == true)
        {
            return false;
        }

        if (targetPos.Count > 0 || evasionPos.Count > 0)
        {
            return false;
        }

        if (_stateCount >= 1f)
        {
            _stateCount = 0f;
            return false;
        }

        _stateCount++;
        return true;
    }

    public override void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {
        _endTiem = UnityEngine.Random.Range(3f, 6f);

        _isActive = true;

        _onCallbackCorditionComparisonAndStateStay(ConditionComparison, OnStayState);
    }

    protected override void OnStayState()
    {
        _time += Time.deltaTime;

        if(_time >= _endTiem)
        {
            OnExitState();
        }
    }

    protected override void OnExitState()
    {
        _isActive = false;
        _time = 0f;
    }

    private void OnDrawGizmos()
    {
        if(_npc == null)
        {
            return;
        }

        if(this._isActive == false)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_npc.transform.position, 1f);
    }
}
