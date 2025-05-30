using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundInfo : MonoBehaviour
{
    
    public int[] shortStorage;  //information that is needed to be transported from the main menu to the simulation scene
    public bool newBed;
    public bool overrule; //overrule the choice of the RedSys Bed because it does not make sense in this context

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);    //is not destroyed when changing scene
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
