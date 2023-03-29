using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Character State - Runaway", menuName = "Character State/Runaway", order = 14)]
public class State_Runaway : State_
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

        //if (character.healthPoint >= character.targetCharacter.healthPoint || character.isUnderAttack == true)
        //{
        //    return false;
        //}

        return true;
    }

    public override void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {
        character.NearestObject(ref objectDetected);

        Vector3 dir = (character.transform.position - character.targetCharacter.transform.position).normalized;
        character.targetPosition = character.transform.position + (dir * character.searchRadius);
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

        float dis = Vector3.Distance(character.transform.position, character.targetPosition);

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

        if (dis <= character.transform.localScale.x * 0.5f)
        {
            OnExitState(character);
        }
    }

    public override void OnExitState(Character character)
    {
        character.TargetReset();
        character.time = 0f;
        character.targetPosition = Vector3.zero;
        character.isAction = false;
    }
}
