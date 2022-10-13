using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureEventProcessor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGestureComplete(GestureCompletionData data) {
        if (data.gestureID < 0) {
            string message = GestureRecognition.getErrorMessage(data.gestureID);
            return;
        }
        if (data.gestureID > 0.5) { 
            ///
        }
    }
}
