using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController _controller = null;

    [SerializeField]
    private float gravity = 0f;

    public void Move(Vector3 moveDir, float movespeed)
    {
        moveDir.y -= gravity;
        _controller.Move(moveDir.normalized * Time.deltaTime * movespeed);
    }
}
