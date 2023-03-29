using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionState : OldState
{
    private float _time = 0f;

    private float _endTiem = 0f;

    public override bool Condition(List<Npc> targetPos, List<Npc> evasionPos, bool isActive)
    {
        if(isActive == true)
        {
            return false;
        }

        if (evasionPos.Count > 0)
        {
            return false;
        }

        if (targetPos.Count == 0)
        {
            return false;
        }

        if(_time != 0)
        {
            return false;
        }

        return true;
    }

    public override void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {
        if (_time != 0)
        {
            return;
        }

        _endTiem = UnityEngine.Random.Range(1f, 3f);

        _isActive = true;

        _onCallbackCorditionComparisonAndStateStay(ConditionComparison, OnStayState);
    }

    protected override void OnStayState()
    {
        if (_isActive == false)
        {
            return;
        }

        _time += Time.deltaTime;

        if (_time >= _endTiem)
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
        if (_npc == null)
        {
            return;
        }

        if (this._isActive == false)
        {
            return;
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_npc.transform.position, 1f);
    }
}
