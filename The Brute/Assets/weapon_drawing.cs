using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon_drawing : MonoBehaviour
{ 
    private managerWeaponChange mngr;

    // Start is called before the first frame update
    void Start()
    {
        mngr = GameObject.FindGameObjectWithTag("Player").GetComponent<managerWeaponChange>();
    }

    public void on_completion(GestureCompletionData data) {
        if (data.gestureID < 0) {
            string msg = GestureRecognition.getErrorMessage(data.gestureID);
            Debug.Log(msg);
        }
        if (data.similarity >= 0.5) {
            if (data.gestureName == "sword") {
                mngr.ChangeWeapon(1);
            }
        }
    }
}
