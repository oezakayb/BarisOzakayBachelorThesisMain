using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour   //check out standardInteractable script for more information
{
    //public Transform PlayerLeft;        //console controllers 
    //public Transform PlayerRight;
    float MaxDistance = 2;
    public Animator anim;
    private bool opened = false;
    public GameObject handle1o;         //different UI windows to interact
    public GameObject handle2o;
    public GameObject handle1c;
    public GameObject handle2c;
    public bool dummy;
    public OcclusionPortal portalOuter;     //portals for occlusion culling
    public OcclusionPortal portalInner;

    // Start is called before the first frame update
    void Start()
    {
        handle1o.gameObject.SetActive(false);
        handle2o.gameObject.SetActive(false);
        handle1c.gameObject.SetActive(false);
        handle2c.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(dummy == false){


            RaycastHit doorhit;
            
            /*
            if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out doorhit, MaxDistance))    //check if the left controller aims at the door and is only in a certain distance
            {

                if (doorhit.transform.tag == "Door")
                {

                    if (opened == false)                        //check the state and adapt UI windows
                    {
                        handle1o.gameObject.SetActive(true);
                        handle2o.gameObject.SetActive(true);
                    }
                    else
                    {
                        handle1c.gameObject.SetActive(true);
                        handle2c.gameObject.SetActive(true);
                    }

                }
            }
            if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out doorhit, MaxDistance))   //check if the right controller aims at the door and is only in a certain distance
            {

                if (doorhit.transform.tag == "Door")
                {

                    if (opened == false)
                    {
                        handle1o.gameObject.SetActive(true);
                        handle2o.gameObject.SetActive(true);
                    }
                    else
                    {
                        handle1c.gameObject.SetActive(true);
                        handle2c.gameObject.SetActive(true);
                    }

                }
            }*/
        }

    }

    private void FixedUpdate()
    {
        if (opened == true && anim.GetInteger("opened") == 1)       //change the state of the animation
        {
            
            anim.SetInteger("opened", 0);
        }
        if (opened == false && anim.GetInteger("opened") == 2)
        {
            anim.SetInteger("opened", 0);
        }
    }

    public void setInactiveAndOpen()                            //everything connected to opening the door
    {
       
        handle1o.gameObject.SetActive(false);
        handle2o.gameObject.SetActive(false);
        opened = true;
        anim.SetInteger("opened", 1);
        this.gameObject.GetComponent<AudioSource>().Play();
        if(portalOuter != null)                                 //the occlusion culling on the other side of the door/portal is stopped
        {
            portalOuter.open = true;
        }
        if (portalInner != null)
        {
            portalInner.open = true;
        }
    }
    public void setInactiveAndClose()                           //everything connected to closing the door
    {
        handle1c.gameObject.SetActive(false);
        handle2c.gameObject.SetActive(false);
        opened = false;
        anim.SetInteger("opened", 2);
        StartCoroutine(closePortal());
    }

    IEnumerator closePortal()                                   //the occlusion culling on the other side of the door/portal resumes after the time needed for the animation has passed
    {
        yield return new WaitForSeconds(1.5f);
        if (portalOuter != null)
        {
            portalOuter.open = false;
        }
        if (portalInner != null)
        {
            portalInner.open = false;
        }
    }
}
