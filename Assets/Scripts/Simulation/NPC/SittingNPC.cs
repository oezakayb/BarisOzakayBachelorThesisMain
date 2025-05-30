using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingNPC : MonoBehaviour             //check out standardInteractable script for more information
{
    //public Transform PlayerLeft;
    //public Transform PlayerRight;
    float MaxDistance = 2;
    public Animator anim;
    public GameObject talk;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetInteger("state", 0);
        talk.gameObject.SetActive(false);
    }


    // Update is called once per frame
    /*
    void Update()
    {
        RaycastHit colhit;
        
        if ((Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out colhit, MaxDistance))|| (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out colhit, MaxDistance)))
        {
            
            if (colhit.transform.tag == "Colleague")
            {
                talk.gameObject.SetActive(true);
                
            }
        }
    }
    */

    public void Talking()               //switch animation state
    {
        
        anim.SetInteger("state", 1);
    }

    public void stopTalking()
    {
        anim.SetInteger("state", 0);
    }
}
