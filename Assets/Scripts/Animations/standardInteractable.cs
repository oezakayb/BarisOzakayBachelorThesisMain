using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class standardInteractable : MonoBehaviour
{
    public Transform PlayerLeft;        //both controllers
    public Transform PlayerRight;
    float MaxDistance = 2;              //maximum distance to interact
    public Animator anim;               //animator
    public GameObject handle;           //UI object that must be used to interact
   

    // Start is called before the first frame update
    void Start()
    {
        handle.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit devicehit;

        if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out devicehit, MaxDistance))   //check if left controller hovers over object
        {

            if (devicehit.transform.tag == "Object")    //correct tag is found
            {
                handle.gameObject.SetActive(true);      //set active UI object
                
            }
        }
        if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out devicehit, MaxDistance))     ////check if right controller hovers over object
        {

            if (devicehit.transform.tag == "Object")
            {
                handle.gameObject.SetActive(true);

            }
        }
    }
}
