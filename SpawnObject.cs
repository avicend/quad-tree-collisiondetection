using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 pos;

    public Vector3 center;
    public Vector3 size;

    public GameObject objectPrefab;


    public GameObject spawnPoint;

    public void Start()
    {

   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPrefab(Rect areaToSpawn)
    {
        center = areaToSpawn.center;
        size = areaToSpawn.size;
        pos = center + new Vector3(Random.Range(-size.x/2 ,size.x/2 ),0,Random.Range(-size.y/2  , size.y/2));

        Instantiate(objectPrefab,pos,Quaternion.identity);          
    }
    
}
