using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorPoint : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent playerAgent;

    public GameObject door;
    private DoorAnim doorAnim;

    public GameObject doorExit;

    private bool doorOpen = false;
    private bool called = false;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAgent = player.GetComponent<NavMeshAgent>();
        doorAnim = door.GetComponent<DoorAnim>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!called)
        {
            if (player.transform.position.x == transform.position.x && player.transform.position.z == transform.position.z)
            {
                called = true;
                StartCoroutine(OpenDoor());

            }
        }

        if (called)
        {
            if (doorOpen)
            {
                playerAgent.destination = doorExit.transform.position;
                player.GetComponent<PlayerController>().destination = doorExit.transform.position;
                if (player.transform.position.x == doorExit.transform.position.x && player.transform.position.z == doorExit.transform.position.z)
                {
                    called = false;
                    doorAnim.setInactiveAndClose();
                    doorOpen = false;

                }
            }
        }
        
    }

    public IEnumerator OpenDoor()
    {
        doorAnim.setInactiveAndOpen();
        yield return new WaitForSeconds(1);
        doorOpen = true;
    }

}
