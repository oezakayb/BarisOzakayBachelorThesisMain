using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanitizerAnim : MonoBehaviour      //check out standardInteractable script for more information
{

    //public Transform PlayerLeft;
    //public Transform PlayerRight;
    float MaxDistance = 2;
    public Animator anim;
    public GameObject handle;

    // Start is called before the first frame update
    void Start()
    {
        handle.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit sanhit;

        /*
        if (Physics.Raycast(PlayerLeft.transform.position, PlayerLeft.transform.forward, out sanhit, MaxDistance))
        {

            if (sanhit.transform.tag == "Sanitizer")
            {
                handle.gameObject.SetActive(true);
                anim.SetInteger("active", 0);
            }
        }
        if (Physics.Raycast(PlayerRight.transform.position, PlayerRight.transform.forward, out sanhit, MaxDistance))
        {

            if (sanhit.transform.tag == "Sanitizer")
            {
                handle.gameObject.SetActive(true);
                anim.SetInteger("active", 0);
            }
        }
        */
    }

    public void Activate()
    {
        anim.SetInteger("active", 1);
        handle.gameObject.SetActive(false);
       
    }

}
