using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IQuadTreeObject
{
    Vector2 GetPosition();
    GameObject GetPrefab();

}


public class QuadTree<T> where T : MonoBehaviour, IQuadTreeObject
{
    private int _maxObjCount;
    private List<T> _storedObjs;
    public Rect _bounds;
    private QuadTree<T>[] _cells;
    public int _maxLevel;
    public int _currentLevel;
    public List<T> _collidedObjects;
    private GameObject _colPrefab;
    private GameObject[] collidedOnes;
    public GameObject[] returnCollideds;
    public int returningCurrent;


    public bool flag
    {
        get;
        set;
    }

    /*
     * Constructor
     */
    public QuadTree(int currentLevel, int maxSize, int maxLevel, Rect bounds, GameObject colPrefab)
    {
        _maxLevel = maxLevel;
        _bounds = bounds;
        _maxObjCount = maxSize;
        _cells = new QuadTree<T>[4];
        _storedObjs = new List<T>(maxSize);
        _currentLevel = currentLevel;
        _colPrefab = colPrefab;
        flag = false;

    }

    public GameObject[] GetCollidedObjects()
    {

        for (int i = 0; i < _collidedObjects.Count; i++)
        {
            returnCollideds[i] = _collidedObjects[i].GetPrefab();
        }

        return returnCollideds;
    }

    /*
    * Assign  certain objects to corresponding leafs. If objects count passes max --->subdivide and assign again
    */
    public void addObjects(T objToAdd)
    {

        if (_cells[0] != null)
        {
            int iniCell = GetCellToInsertObject(objToAdd.GetPosition());

            if (iniCell > -1)
            {
                _cells[iniCell].addObjects(objToAdd);
            }
            return;
        }


        _storedObjs.Add(objToAdd);

        if (_storedObjs.Count >= _maxObjCount && _currentLevel < _maxLevel)//Subdivide the quad to 4
        {


            if (_cells[0] == null)
            {
                float subWidth = (_bounds.width / 2f);
                float subHeight = (_bounds.height / 2f);
                float x = _bounds.x;
                float y = _bounds.y;
                _cells[0] = new QuadTree<T>(_currentLevel + 1, _maxObjCount, _maxLevel, new Rect(x + subWidth, y, subWidth, subHeight), _colPrefab);
                _cells[1] = new QuadTree<T>(_currentLevel + 1, _maxObjCount, _maxLevel, new Rect(x, y, subWidth, subHeight), _colPrefab);
                _cells[2] = new QuadTree<T>(_currentLevel + 1, _maxObjCount, _maxLevel, new Rect(x, y + subHeight, subWidth, subHeight), _colPrefab);
                _cells[3] = new QuadTree<T>(_currentLevel + 1, _maxObjCount, _maxLevel, new Rect(x + subWidth, y + subHeight, subWidth, subHeight), _colPrefab);


            }

            //Assign objects to new children leafs
            int i = _storedObjs.Count - 1; ;

            while (i >= 0)
            {
                T storedObj = _storedObjs[i];

                int iCell = GetCellToInsertObject(storedObj.GetPosition());
                if (iCell > -1)
                {
                    _cells[iCell].addObjects(storedObj);

                }
                _storedObjs.RemoveAt(i);
                i--;

            }
        }
        else if (_currentLevel == _maxLevel)
        {
            flag = true;
            _collidedObjects = RetrieveObjectsInArea(_bounds);

            if (_collidedObjects.Count >= 2)
            {
                
                for (int i = 0; i < _collidedObjects.Count; i++)
                {
                    _collidedObjects[i].GetPrefab().GetComponent<Renderer>().material.color = Color.red;


                }



            }


        }
    }




    /*
    * Removes an object from its cell
    */
    public void objRemove(T objectToRemove)
    {

        if (ContainsLocation(objectToRemove.GetPosition()))
        {

            _storedObjs.Remove(objectToRemove);

            if (_cells[0] != null)
            {
                for (int i = 0; i < 4; i++)
                {

                    _cells[i].objRemove(objectToRemove);
                }
            }
        }
    }


    /*
    * Get the object on defined area
    */
    public List<T> RetrieveObjectsInArea(Rect area)
    {

        if (rectOverlap(_bounds, area))
        {
            List<T> returnedObjects = new List<T>();
            for (int i = 0; i < _storedObjs.Count; i++)
            {
                if (area.Contains(_storedObjs[i].GetPosition()))
                {
                    returnedObjects.Add(_storedObjs[i]);
                }
            }
            if (_cells[0] != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    List<T> cellObjects = _cells[i].RetrieveObjectsInArea(area);
                    if (cellObjects != null)
                    {
                        returnedObjects.AddRange(cellObjects);
                    }
                }
            }
            return returnedObjects;
        }
        return null;
    }

    /*
    * Clear quadtree
    */
    public void ClearTheTree()
    {
        _storedObjs.Clear();


        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i] != null)
            {
                _cells[i].ClearTheTree();
                _cells[i] = null;
            }
        }
    }


    /*
    * Return cell that object needs to be inserted
    */
    private int GetCellToInsertObject(Vector2 location)
    {

        for (int i = 0; i < 4; i++)
        {
            if (_cells[i].ContainsLocation(location))
            {
                return i;
            }
        }
        return -1;
    }

    /*
    * 
    */
    public bool ContainsLocation(Vector2 location)
    {
        return _bounds.Contains(location);
    }

    /*
    * Check overlapping areas
    */
    bool rectOverlap(Rect A, Rect B)
    {
        bool xOverlap = valueInRange(A.x, B.x, B.x + B.width) ||
                        valueInRange(B.x, A.x, A.x + A.width);

        bool yOverlap = valueInRange(A.y, B.y, B.y + B.height) ||
                        valueInRange(B.y, A.y, A.y + A.height);

        return xOverlap && yOverlap;
    }

    /*
    * Return the value in range
    */
    bool valueInRange(float value, float min, float max)
    {
        return (value >= min) && (value <= max);
    }


    /*
    * Draw the quadtree in order to debug and see certain problems
    */
    public void DrawDebug()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(new Vector3(_bounds.x, 0, _bounds.y), new Vector3(_bounds.x, 0, _bounds.y + _bounds.height));
        Gizmos.DrawLine(new Vector3(_bounds.x, 0, _bounds.y), new Vector3(_bounds.x + _bounds.width, 0, _bounds.y));
        Gizmos.DrawLine(new Vector3(_bounds.x + _bounds.width, 0, _bounds.y), new Vector3(_bounds.x + _bounds.width, 0, _bounds.y + _bounds.height));
        Gizmos.DrawLine(new Vector3(_bounds.x, 0, _bounds.y + _bounds.height), new Vector3(_bounds.x + _bounds.width, 0, _bounds.y + _bounds.height));
        if (_cells[0] != null)
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i] != null)
                {
                    _cells[i].DrawDebug();
                }
            }
        }
    }
}
