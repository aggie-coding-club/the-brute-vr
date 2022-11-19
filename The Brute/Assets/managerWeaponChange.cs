using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managerWeaponChange : MonoBehaviour
{
    public Transform pivotR;
    private managerWeapon mngrWpn;

    // Start is called before the first frame update
    void Start()
    {
        mngrWpn = GameObject.Find("managerWeapon").getComponent<managerWeapon>();

        GameObject tempDefaultWeapon = mngrWpn.weapons[0];
        Instantiate(tempDefaultWeapon, pivotR);
        previousIndex = 0;
    }

    public void ChangeWeapon(int index) {
        if (index != previousIndex) {
            Destroy(pivotR.GetChild(0).gameObject);
            GameObject tempWeapon = mngrWpn.weapons[index];
            Instantiate(tempWeapon, pivotR);

            previousIndex = index;
        }
    }
}
