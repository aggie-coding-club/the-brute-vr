using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    public InputActionProperty pinch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gestureCompletion(GestureCompletionData data) {
        if (data.gestureID < 0) {
            string msg = GestureRecognition.getErrorMessage(data.gestureID);
            Debug.Log(msg);
            return;
        }
        if (data.similarity >= 0.5) { //means a gesture has been recognized (according to doc)
            if (data.gestureName == "Test1") { //do the pinch thing with the hands
                return;
            }
        }
    }
}
