using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponCollision : MonoBehaviour
{
    public ParticleSystem collisionParticleSystem;
    // public Graphics gr;
    public bool once = true;

    void OnCollisionEnter (Collision collisionInfo) {
        if( collisionInfo.gameObject.CompareTag("weapon") && once) {

            Debug.Log("We hit!!!!");

            var em = collisionParticleSystem.emission;
            var dur = collisionParticleSystem.duration;

            em.enabled = true;
            collisionParticleSystem.Play();

            // Destroy(gr);
            // Invoke(nameof(DestroyObj), dur);
        }

    }

    // void DestroyObj() {
    //     Destroy(gameObject);
    // }
}
