using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingNPC : MonoBehaviour
{
    public GameObject npc;
    public Animator anim;
    public int startState;
    public bool inMovement;

    public GameObject Destination;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetInteger("state", startState);
        inMovement = false;
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(noWalkingYet());
    }

    // Update is called once per frame
    void Update()
    {
        if(inMovement == true)                  //set destination when allowed to move
        {
            agent.SetDestination(Destination.transform.position);
        }

    }

    public void stopMoving()                    //called by npcDest when destination is reached
    {
        inMovement = false;
        anim.SetInteger("state", 0);
        agent.SetDestination(npc.gameObject.transform.position);
    }

    IEnumerator noWalkingYet()                  //delay before walking
    {
        yield return new WaitForSeconds(10);
        anim.SetInteger("state", 1);
        inMovement = true;
    }
}
