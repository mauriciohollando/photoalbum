using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchViewButton : MonoBehaviour
{
    public GameObject[] Views;
    public int CurrentViewIdx;
    
    void Start()
    {
        Views[1].SetActive(false);
    }
    
    public void SwitchView()
    {
        Views[CurrentViewIdx].SetActive(false);
        CurrentViewIdx = 1 - CurrentViewIdx;
        Views[CurrentViewIdx].SetActive(true);
        
    }
}
