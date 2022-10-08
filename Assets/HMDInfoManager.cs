using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HMDInfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Is Device Active " + XRSettings.isDeviceActive);
        Debug.Log("Device Name is " + XRSettings.loadedDeviceName);

        if(!XRSettings.isDeviceActive) {
            Debug.Log("No headset plugged in");
        }
        else if(XRSettings.isDeviceActive && XRSettings.loadedDeviceName == "MockHMD Display") {
            Debug.Log("Using MockHMD");
        }
        else {
            Debug.Log("We have a headset " + XRSettings.loadedDeviceName);
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
