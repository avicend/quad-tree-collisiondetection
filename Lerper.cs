using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerper : MonoBehaviour
{

    public Transform startPos, endPos;
    public bool repeatable = false;
    public float speed = 1.0f;
    public float duration = 3.0f;

    float startTime, totalDistance;

    public Rect areaToMove;
    public GameObject spawnQuadRect;
    public Vector3 pos;

    public Vector3 center;
    public Vector3 size;

    private void Awake()
    {
        areaToMove = spawnQuadRect.GetComponent<QuadTreeTest>().quadTree._bounds;
        spawnQuadRect = GameObject.FindGameObjectWithTag("GameController");
    }
    // Use this for initialization
    IEnumerator Start()
    {
        

        center = areaToMove.center;
        size = areaToMove.size;
        pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(-size.y / 2, size.y / 2));
        endPos.position = pos;

        startTime = Time.time;
        totalDistance = Vector3.Distance(startPos.position, endPos.position);
        while (repeatable)
        {
            yield return RepeatLerp(startPos.position, endPos.position, duration);
            yield return RepeatLerp(endPos.position, startPos.position, duration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!repeatable)
        {
            float currentDuration = (Time.time - startTime) * speed;
            float journeyFraction = currentDuration / totalDistance;
            this.transform.position = Vector3.Lerp(startPos.position, endPos.position, journeyFraction);
        }
    }

    public IEnumerator RepeatLerp(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            this.transform.position = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }
}