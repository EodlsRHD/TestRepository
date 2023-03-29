using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    enum eMoveDir
    {
        left,
        right,
        stop,
        keybord
    }

    [SerializeField]
    private Player _player = null;

    private float _cornerDegree = 0f;

    private Vector3 moveDir = Vector3.zero;

    [SerializeField]
    [Range(40, 1000)]
    private float movespeed = 40f;

    [SerializeField]
    private eMoveDir _moveDir = eMoveDir.right;

    private void Update()
    {
        SetMode(_moveDir);
        this.transform.position = _player.transform.position;
    }

    void SetMode(eMoveDir _moveDir)
    {
        switch(_moveDir)
        {
            case eMoveDir.right:
                moveDir = (-this.transform.right); new Vector3(1f, 0f,0f);
                _player.transform.rotation = Quaternion.Lerp(_player.transform.rotation, Quaternion.LookRotation(moveDir) * Quaternion.Euler(new Vector3(0f, -90f, 0f)), 100f);
                break;

            case eMoveDir.left:
                moveDir = (this.transform.right);
                _player.transform.rotation = Quaternion.Lerp(_player.transform.rotation, Quaternion.LookRotation(moveDir) * Quaternion.Euler(new Vector3(0f, -90f, 0f)), 100f);
                break;

            case eMoveDir.stop:
                moveDir = Vector3.zero;
                _player.transform.rotation = Quaternion.Lerp(_player.transform.rotation, Quaternion.LookRotation(moveDir) * Quaternion.Euler(new Vector3(0f, -90f, 0f)), 100f);
                break;

            case eMoveDir.keybord:
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                if ((h == 0 && v == 0) == false)
                {
                    _player.transform.rotation = Quaternion.Lerp(_player.transform.rotation, Quaternion.LookRotation(moveDir) * Quaternion.Euler(new Vector3(0f, -90f, 0f)), 100f);
                }
                moveDir = new Vector3(h, 0f, v);
                return;
        }
        _player.Move(moveDir, movespeed);
    }

    public void SetCornerCamera(float cornerDegree)
    {
        _cornerDegree = cornerDegree;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(this.transform.rotation.x, _cornerDegree, this.transform.rotation.z), _cornerDegree);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.right);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (-this.transform.right));
    }
}
