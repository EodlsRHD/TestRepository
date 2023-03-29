using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMeshAgent = null;

    [SerializeField]
    private float _speed = 0f;

    [SerializeField]
    private float _interactionDistance = 0f;

    [SerializeField]
    private float _searchRadius = 30f;

    public NavMeshAgent navMeshAgent
    {
        get { return _navMeshAgent; }
    }

    public float speed
    {
        get { return _speed; }
    }

    public float interactionDistance
    {
        get { return _interactionDistance; }
    }

    public float searchRadius
    {
        get { return _searchRadius; }
    }

    private List<Npc> tagOnes = new List<Npc>();

    private List<Npc> tagTwos = new List<Npc>();

    private void Start()
    {
        _navMeshAgent.speed = _speed;
    }

    public void AroundSearch(string tagOne, string tagTwo, System.Action<List<Npc>, List<Npc>> onCallbackResult)
    {
        tagOnes.Clear();
        tagTwos.Clear();

        if(tagOne.Contains("Non"))
        {
            tagOne = string.Empty;
        }

        if(tagTwo.Contains("Non"))
        {
            tagTwo = string.Empty;
        }

        Collider[] colls = Physics.OverlapSphere(this.transform.position, _searchRadius);

        foreach(var coll in colls)
        {
            if(tagOne != string.Empty)
            {
                if (coll.gameObject.CompareTag(tagOne))
                {
                    coll.gameObject.TryGetComponent<Npc>(out Npc npc);
                    tagOnes.Add(npc);
                }
            }

            if(tagTwo != string.Empty)
            {
                if (coll.gameObject.CompareTag(tagTwo))
                {
                    coll.gameObject.TryGetComponent<Npc>(out Npc npc);
                    tagTwos.Add(npc);
                }
            }
        }

        onCallbackResult(tagOnes, tagTwos);
    }
}
