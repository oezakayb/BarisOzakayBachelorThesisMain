using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetParentName : MonoBehaviour
{
    
    public TextMeshProUGUI textObject;

    // Start is called before the first frame update
    void Start()
    {
        
        textObject.GetComponent<TextMeshProUGUI>().text = this.name;
        
    }

}
