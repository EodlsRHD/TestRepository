using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCharacterInfo", menuName = "Character State/CharacterInfo", order = 2)]
public class CharacterInfo : ScriptableObject
{
    [Header("Stat")]
    public float speed;

    public float damage;

    public float healthPoint;

    [Header("Info")]
    public float interactionDistance;

    public float searchRadius;

    public float walkRadius;

    public float attackRate;
}
