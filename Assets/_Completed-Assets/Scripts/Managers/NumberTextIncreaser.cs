using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberTextIncreaser : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    int counter;

    public void ButoonPressed()
    {
        counter++;
        numberText.text = counter + "";
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
