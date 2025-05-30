using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDeviceAnim : MonoBehaviour     //check out standardInteractable script for more information
{
    public Transform PlayerLeft;
    public Transform PlayerRight;
    //float MaxDistance = 2;
    public Animator anim;
    public GameObject handle;
    public GameObject handle2;

    // Start is called before the first frame update
    void Start()
    {
        handle.gameObject.SetActive(false);
        handle2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()           //not used right now
    {
        /*RaycastHit devicehit;

        if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out devicehit, MaxDistance))
        {

            if (devicehit.transform.tag == "Device")
            {
                handle.gameObject.SetActive(true);
                
            }
        }
        if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out devicehit, MaxDistance))
        {

            if (devicehit.transform.tag == "Device")
            {
                handle.gameObject.SetActive(true);

            }
        }*/
    }

    public void pullDevice()    
    {
        anim.SetInteger("state", 0);
    }

    public void pushDevice()
    {
        anim.SetInteger("state", 1);
        handle2.gameObject.SetActive(true);
    }
}
