using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuadTreeTest : MonoBehaviour
{
    public class TestObject : MonoBehaviour, IQuadTreeObject
    {
        private Vector3 _vPosition;
        public GameObject _prefObj;

        public TestObject(Vector3 position, GameObject prefabObject)
        {
            _vPosition = position;
            _prefObj = prefabObject;

        }
        public Vector2 GetPosition()
        {           
            return new Vector2(_vPosition.x, _vPosition.z);
        }

        public GameObject GetPrefab()
        {
            return _prefObj;
        }

    }
    public QuadTree<TestObject> quadTree;
    public GameObject[] clones;
    public GameObject clonePrefab;
    public GameObject collisionPref;
    ObjectPooler objectPooler;
    public GameObject[] _collidedOnes;

    private int currentLevelTree;
    private int maxLevelTree;

    public SpawnObject randomSpawner;

    public int desiredObjects;

    private bool _isAllSpawned = false;

    public InputField objectCountByUser;
    public Toggle treeOnOff;
    public bool isDebugEnabled = false;

    public void SetObjectCount()
    {
        //clones = GameObject.FindGameObjectsWithTag("Clone");

        int objcount = int.Parse(objectCountByUser.text);
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
            clones[i].SetActive(false);
        }
        desiredObjects = objcount;



        _isAllSpawned = false;
    }

    public void ToggleDebug()
    {
        if (isDebugEnabled)
        {
            isDebugEnabled = false;
        }
        else
            isDebugEnabled = true;
    }



    private void OnEnable()
    {
        randomSpawner = GetComponent<SpawnObject>();
        quadTree = new QuadTree<TestObject>(0, 2, 5, new Rect(-100, -100, 200, 200), collisionPref);
        objectPooler = this.GetComponent<ObjectPooler>();


    }
    private void Start()
    {

    }
    void Update()
    {

        quadTree.ClearTheTree();
        clones = GameObject.FindGameObjectsWithTag("Clone");


        for (int i = 0; i < clones.Length; i++)
        {

            clones[i].GetComponent<Renderer>().material.color = Color.white;

            TestObject newObject = new TestObject(clones[i].GetComponent<Transform>().position, clones[i].gameObject);

            quadTree.addObjects(newObject);

        }


        if (!_isAllSpawned)
        {
            for (int i = 0; i < desiredObjects; i++)
            {
                randomSpawner.SpawnPrefab(quadTree._bounds);

            }
            _isAllSpawned = true;
        }

    }



    void OnDrawGizmos()
    {
        if (quadTree != null && isDebugEnabled)
        {
            quadTree.DrawDebug();
        }
    }





}
