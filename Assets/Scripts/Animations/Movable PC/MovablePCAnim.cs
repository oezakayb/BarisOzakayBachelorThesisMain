using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePCAnim : MonoBehaviour      //check out standardInteractable script for more information
{
    //public Transform PlayerLeft;
    //public Transform PlayerRight;
    float MaxDistance = 2;
    public Animator anim;
    public GameObject handle;
    public GameObject door;
    bool moved = false;
    //public GameObject handle2;

    // Start is called before the first frame update
    void Start()
    {
        handle.gameObject.SetActive(false);
        //handle2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit devicehit;
        
        /*
        if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out devicehit, MaxDistance))
        {

            if (devicehit.transform.tag == "MovablePC" && moved == false)
            {
                handle.gameObject.SetActive(true);
                
            }
        }
        if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out devicehit, MaxDistance))
        {

            if (devicehit.transform.tag == "MovablePC" && moved == false)
            {
                handle.gameObject.SetActive(true);

            }
        }
        */
    }

    public void pushDevice()        //device moved to the other room
    {
        moved = true;
        anim.speed = 0.4f;
        anim.SetInteger("state", 1);
        handle.gameObject.SetActive(false);
        door.gameObject.GetComponent<DoorAnim>().setInactiveAndOpen();
    }
}
