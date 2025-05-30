using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailingAnim : MonoBehaviour        //check out standardInteractable script for more information
{
    //public Transform PlayerLeft;
    //public Transform PlayerRight;
    float MaxDistance = 2;
    public Animator anim;
    public GameObject handle;
    bool pulled = false;

    // Start is called before the first frame update
    void Start()
    {
        handle.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit railhit;

        /*
        if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out railhit, MaxDistance))
        {

            if (railhit.transform.tag == "Railing" && pulled == false)
            {
                handle.gameObject.SetActive(true);
                
            }
        }
        if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out railhit, MaxDistance))
        {

            if (railhit.transform.tag == "Railing" && pulled == false)
            {
                handle.gameObject.SetActive(true);

            }
        }
        */
    }

    public void pullRailing()
    {
        anim.SetInteger("state", 1);
        pulled = true;
    }

    public void pushRailing()
    {
        anim.SetInteger("state", 0);
        pulled = false;
    }
}
