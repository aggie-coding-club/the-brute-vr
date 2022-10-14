using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandAnimator : MonoBehaviour
{
    public Animator handAnimator; 

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
            if (data.gestureName == "test1") { //do the pinch thing with the hands
                float val = 1;
                handAnimator.SetFloat("Trigger", val);
            }
        }
    }
}
