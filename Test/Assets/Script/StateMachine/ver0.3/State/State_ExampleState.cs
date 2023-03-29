using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "Character State - (enter state name)", menuName = "Character State/(enter state name)", order = (start number is 12))]
public class State_ExampleState : State_
{
    public override bool Condition(ref Character character, ref List<Character> objectDetected)
    {
        // write to return false

        return true;
    }

    public override void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {
        // write

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

        // Write to exit state condition
    }

    public override void OnExitState(Character character)
    {
        character.TargetReset();

        // Write variable to initialize
    }
}
