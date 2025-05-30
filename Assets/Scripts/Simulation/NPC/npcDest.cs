using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcDest : MonoBehaviour
{
    public int pivotPoint;
    public int[] destPos; //list of different destinations an npc can have: x y z

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)     
    {
       
        if(other.tag == "NPC")  //when destination is reached by NPC
        {
           
            if(pivotPoint == 0) //if npc has second destination
            {
                
            }

            other.gameObject.GetComponent<WalkingNPC>().stopMoving();                    //stop their animation
            other.gameObject.transform.eulerAngles = new Vector3(0, 45, 0);              //rotate them
            other.gameObject.GetComponent<WalkingNPC>().anim.SetInteger("state", 2);     //go into state where they point forward regularly
        }
    }
}
