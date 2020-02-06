using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Object : MonoBehaviour, IPooledObject
{
    public int health = 5;
    public bool isRed = false;
    public GameObject spawnOnDeath;
    public GameObject spawnQuadRect;
    public float objScale;
    private Rect areaToSpawn;
    private SpawnObject _objGoingtToSpawn;
    public ObjectPooler enqueuePooler;

    private Vector3 pos;

    private Vector3 center;
    private Vector3 size;
    private Vector3 startPos, endPos;

    private float timer;
    private float curTime;
    private float speed;
    private Vector3 oldDir;
    private bool flag = false; 
    public QuadTreeTest runtime;




    // Start is called before the first frame update
    public void Start()
    {



        spawnOnDeath = GameObject.FindGameObjectWithTag("GameController");
        spawnQuadRect = GameObject.FindGameObjectWithTag("GameController");

        objScale = Random.Range(gameObject.transform.localScale.x, gameObject.transform.localScale.x * 3);

        gameObject.transform.localScale = new Vector3(objScale, objScale, objScale);
        areaToSpawn = spawnQuadRect.GetComponent<QuadTreeTest>().quadTree._bounds;
        _objGoingtToSpawn = spawnOnDeath.GetComponent<SpawnObject>();

        center = areaToSpawn.center;
        size = areaToSpawn.size;
        pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(-size.y / 2, size.y / 2));


        endPos = _objGoingtToSpawn.pos;



    }

    public void OnObjectSpawn()
    {

    }



    // Update is called once per frame
    void Update()
    {

        if (!isRed && GetComponent<Renderer>().material.color == Color.red)
        {
            health--;
            isRed = true;
        }

        if (GetComponent<Renderer>().material.color == Color.white)
        {
            isRed = false;
        }
        if (health == 0)
        {
            Destroy(this);
            this.gameObject.SetActive(false);
         
            _objGoingtToSpawn.SpawnPrefab(areaToSpawn);

        }



        if (transform.position == endPos)
        {

            oldDir = endPos;
            endPos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(-size.y / 2, size.y / 2));
            timer = 0f;
        }

        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(oldDir, endPos, timer / 2);
    }

}
