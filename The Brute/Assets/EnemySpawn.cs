using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    public Camera camera;
    public int spawnRate = 5;
    private int enemyNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnNow", 1, spawnRate);
    }

    Vector3 getRandomPosition() {
        // Subject to change
        float _x = Random.Range(4f, 10f);
        float _z = Random.Range(-29f, -20f);
        float _y = 7f;

        Vector3 newPos = new Vector3(_x, _y, _z);
        return newPos;
    }

    void SpawnNow() {
        GameObject newEnemy = Instantiate(enemy, getRandomPosition(), Quaternion.identity);
        enemyNum++;
        newEnemy.name = "Enemy" + enemyNum;
        if(I_Can_See(newEnemy)) {
            Destroy(newEnemy);
            enemyNum--;
            Debug.Log("Enemy spawned within player view"); 
        }
    }

    private bool I_Can_See(GameObject Object) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        if (GeometryUtility.TestPlanesAABB(planes, Object.GetComponent<Collider>().bounds))
            return true;
        else
            return false;
    }
}
