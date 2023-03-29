using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunawayState : OldState
{
    private Vector3 _target = Vector3.zero;

    public override bool Condition(List<Npc> targetPos, List<Npc> evasionPos, bool isActive)
    {
        if (evasionPos.Count == 0)
        {
            return false;
        }

        return true;
    }

    public override void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {
        float MeanX = 0f;
        float MeanZ = 0f;

        foreach(var target in evasionPos)
        {
            MeanX += target.transform.position.x;
            MeanZ += target.transform.position.z;
        }

        Vector3 averagePos = new Vector3(MeanX, _npc.transform.position.y, MeanZ);
        Vector3 dir = (_npc.transform.position - averagePos).normalized;
        _target = _npc.transform.position + (dir * _npc.searchRadius);

        //if (NavMesh.SamplePosition(_npc.transform.position + (dir * (_npc.searchRadius * 2)), out NavMeshHit hit, _npc.transform.localScale.x, NavMesh.AllAreas) == false)
        //{
        //    ReSearch(_npc.transform.position + (dir * (_npc.searchRadius * 2)));
        //}

        _npc.navMeshAgent.SetDestination(_target);

        _isActive = true;

        _onCallbackCorditionComparisonAndStateStay(ConditionComparison, OnStayState);
    }

    private void ReSearch(Vector3 radarDir)
    {
        //Vector3.Reflect
    }

    protected override void OnStayState()
    {
        if (_isActive == false)
        {
            return;
        }

        float dis = Vector3.Distance(_npc.transform.position, _target);

        if (dis <= _npc.transform.localScale.x * 0.5f)
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

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_npc.transform.position, 1f);
        Gizmos.DrawLine(_target, _npc.transform.position);
        Gizmos.DrawSphere(_target, 0.5f);
    }
}
