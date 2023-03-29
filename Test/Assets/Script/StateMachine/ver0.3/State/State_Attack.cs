using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Character State - Attack", menuName = "Character State/Attack", order = 13)]
public class State_Attack : State_
{
    public override bool Condition(ref Character character, ref List<Character> objectDetected)
    {
        if (character.isAction == true)
        {
            return false;
        }

        if (objectDetected.Count == 0)
        {
            return false;
        }

        if (character.distanceFromTarget > character.interactionDistance)
        {
            return false;
        }

        //if (character.healthPoint < character.targetCharacter.healthPoint)
        //{
        //    if (character.isUnderAttack == true)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        return true;
    }

    public override void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {
        character.NearestObject(ref objectDetected);

        Vector3 dir = (character.targetCharacter.transform.position - character.transform.position).normalized;
        character.targetPosition = character.transform.position + (dir * character.distanceFromTarget);

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

        character.time += Time.deltaTime;

        if (character.time >= character.attackRate)
        {
            character.targetCharacter.Hit(character, character.damage);

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
