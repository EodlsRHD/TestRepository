using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingState : OldState
{
    private Vector3 _target = Vector3.zero;

    private bool _isToClose = false;

    public override bool Condition(List<Npc> targetPos, List<Npc> evasionPos, bool isActive)
    {
        if (evasionPos.Count > 0)
        {
            return false;
        }

        if (targetPos.Count == 0)
        {
            return false;
        }

        //if (_isToClose == true)
        //{
        //    return false;
        //}

        return true;
    }

    public override void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {
        float dis = _npc.searchRadius;
        int target = 0;
        for (int i = 0; i < targetPos.Count; i++)
        {
            if (dis > Vector3.Distance(targetPos[i].transform.position, _npc.transform.position))
            {
                dis = Vector3.Distance(targetPos[i].transform.position, _npc.transform.position);
                target = i;
            }
        }

        dis -= _npc.interactionDistance;

        if (dis > 0.5f)
        {
            _isToClose = false;
        }

        Vector3 dir = (targetPos[target].transform.position - _npc.transform.position).normalized;
        _target = _npc.transform.position + (dir * dis);

        _npc.navMeshAgent.SetDestination(_target);

        _isActive = true;

        _onCallbackCorditionComparisonAndStateStay(ConditionComparison, OnStayState);
    }

    protected override void OnStayState()
    {
        if (_isActive == false)
        {
            return;
        }

        float dis = Vector3.Distance(_target, _npc.transform.position);

        if (dis <= 0.5f)
        {
            _isToClose = true;
            OnExitState();
        }
    }

    protected override void OnExitState()
    {
        _isActive = false;
        _target = Vector3.zero;
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

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_npc.transform.position, 1f);
        Gizmos.DrawLine(_target, _npc.transform.position);
        Gizmos.DrawSphere(_target, 0.5f);
    }
}
