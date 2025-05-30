using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour    //displaying current FPS on the screen on the field of view
{
    public TextMeshProUGUI window;  //window in field of view
    private float count;

    private IEnumerator Start()
    {
        
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;  
            int fps = (int)Mathf.Round(count);
            window.text = fps.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
