using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Character State - Tracking", menuName = "Character State/Tracking", order = 15)]
public class State_Tracking : State_
{
    public override bool Condition(ref Character character, ref List<Character> objectDetected)
    {
        if (objectDetected.Count == 0)
        {
            return false;
        }

        if (character.targetCharacter == null)
        {
            return false;
        }

        if (character.distanceFromTarget <= character.interactionDistance)
        {
            return false;
        }

        //if (character.healthPoint < character.targetCharacter.healthPoint || character.isUnderAttack == false)
        //{
        //    return false;
        //}

        return true;
    }

    public override void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {
        character.NearestObject(ref objectDetected);

        Vector3 dir = (character.targetCharacter.transform.position - character.transform.position).normalized;
        character.targetPosition = character.transform.position + (dir * (character.distanceFromTarget - character.interactionDistance));
        character.targetPosition = new Vector3(character.targetPosition.x, character.transform.position.y, character.targetPosition.z);

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

        if (dis <= 0.5f)
        {
            character.navMeshAgent.SetDestination(character.targetPosition);
            OnExitState(character);
        }
    }

    public override void OnExitState(Character character)
    {
        character.TargetReset();
        character.targetPosition = Vector3.zero;
        character.isAction = false;
    }
}
