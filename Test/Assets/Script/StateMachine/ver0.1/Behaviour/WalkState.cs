using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkState : OldState
{
    private Vector3 _target = Vector3.zero;

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

        return true;
    }

    public override void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {
        if (_target != Vector3.zero)
        {
            return;
        }

        Vector3 randomPos = _npc.transform.position + UnityEngine.Random.insideUnitSphere * _npc.searchRadius;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, _npc.searchRadius, NavMesh.AllAreas))
        {
            _target = hit.position;

            _npc.navMeshAgent.SetDestination(_target);

            _isActive = true;

            _onCallbackCorditionComparisonAndStateStay(ConditionComparison, OnStayState);
        }
    }

    protected override void OnStayState()
    {
        if(_isActive == false)
        {
            return;
        }

        float dis = Vector3.Distance(_target, _npc.transform.position); 

        if(dis <= _npc.transform.localScale.y)
        {
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

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_npc.transform.position, 1f);
        Gizmos.DrawLine(_target, _npc.transform.position);
        Gizmos.DrawSphere(_target, 0.5f);
    }
}
