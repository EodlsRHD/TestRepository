using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[CreateAssetMenu(fileName = "Character State - Walk", menuName = "Character State/Walk", order = 16)]
public class State_Walk : State_
{
    public override bool Condition(ref Character character, ref List<Character> objectDetected)
    {
        if (character.isAction == true)
        {
            return false;
        }

        if (objectDetected.Count > 0)
        {
            return false;
        }

        return true;
    }

    public override void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {
        if (character.targetPosition != Vector3.zero)
        {
            return;
        }

        Vector3 randomPos = character.transform.position + UnityEngine.Random.insideUnitSphere * character.walkRadius;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, character.searchRadius, NavMesh.AllAreas))
        {
            character.targetPosition = hit.position;
        }
        else
        {
            character.targetPosition = character.transform.position;
        }

        character.navMeshAgent.SetDestination(character.targetPosition);

        character.isAction = true;

        onCallbackSetUpdate(OnStayState);
    }

    public override void OnStayState(Character character, Action onCallbackConditionComparison)
    {
        if (character.isAction == false)
        {
            return;
        }

        onCallbackConditionComparison();

        float dis = Vector3.Distance(character.targetPosition, character.transform.position);

        if (character.time == 0f)
        {
            character.oneSecBeforePosition = character.transform.position;
        }

        character.time += Time.deltaTime;

        if (character.time >= 1f)
        {
            character.time = 0f;

            if (Vector3.Distance(character.oneSecBeforePosition, character.transform.position) < 1f)
            {
                OnExitState(character);
                return;
            }
        }

        if (dis <= character.transform.localScale.y)
        {
            OnExitState(character);
        }
    }

    public override void OnExitState(Character character)
    {
        character.TargetReset();
        character.time = 0;
        character.isAction = false;
        character.oneSecBeforePosition = Vector3.zero;
        character.targetPosition = Vector3.zero;
    }
}