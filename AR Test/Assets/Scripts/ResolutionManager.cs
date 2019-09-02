using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public int startWidth, startHeight, targetWidth, targetHeight; 

    void SetResolution()
    {
        Screen.SetResolution(targetWidth, targetHeight, false);
    }
    private void Start()
    {
        SetResolution();
    }
}
