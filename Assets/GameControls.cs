using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour {

    public GameObject blackMokuTemplate, whiteMokuTemplate;

    public bool isBlacksTurn = true;

    private Vector2Int U = Vector2Int.up, D = Vector2Int.down, L = Vector2Int.left, R = Vector2Int.right;

    private Dictionary<Vector2Int, GameObject> blackMoku = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> whiteMoku = new Dictionary<Vector2Int, GameObject>();

	// Use this for initialization
	void Start () {

        GameObject[] existingStones = GameObject.FindGameObjectsWithTag("Moku");
        for (int i = 0; i < existingStones.Length; i++)
        {
            Vector3 existingStoneLocation = existingStones[i].transform.position;
            Vector2Int boardPos = new Vector2Int(Mathf.RoundToInt(existingStoneLocation.x), Mathf.RoundToInt(existingStoneLocation.y));

            (existingStones[i].name.StartsWith("B") ? blackMoku : whiteMoku).Add(boardPos, existingStones[i]);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int p = new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));

            if (ValidAndEmpty(p))
            {
                GameObject newMoku = GameObject.Instantiate(isBlacksTurn ? blackMokuTemplate : whiteMokuTemplate);
                newMoku.transform.position = new Vector3(p.x, p.y, 0);

                (isBlacksTurn ? blackMoku : whiteMoku).Add(p, newMoku);

                DateTime removalStart = DateTime.Now;
                RemoveCapturedStones();
                print("Removal in " + (DateTime.Now - removalStart).TotalMilliseconds + " ms");

                isBlacksTurn = !isBlacksTurn;
            }
        }
	}

    private void RemoveCapturedStones()
    {
        Dictionary<Vector2Int, GameObject> opponentMoku = new Dictionary<Vector2Int, GameObject>(), source = (isBlacksTurn ? whiteMoku : blackMoku);
        foreach (Vector2Int p in source.Keys) opponentMoku.Add(p, source[p]);
        while (opponentMoku.Count > 0)
        {
            MokuGroup group = new MokuGroup(opponentMoku);

            bool groupCanBreathe = false;
            foreach (GameObject moku in group.mokuInGroup.Values) if (MokuCanBreathe(moku)) groupCanBreathe = true;

            if (!groupCanBreathe)
                foreach (Vector2Int p in group.mokuInGroup.Keys)
                {
                    Destroy(group.mokuInGroup[p]);
                    source.Remove(p);
                }
        }
    }

    bool MokuCanBreathe(GameObject moku)
    {
        return ValidAndEmpty(MokuGroup.MokuPos(moku) + U) || ValidAndEmpty(MokuGroup.MokuPos(moku) + D) || ValidAndEmpty(MokuGroup.MokuPos(moku) + L) || ValidAndEmpty(MokuGroup.MokuPos(moku) + R);
    }

    bool ValidAndEmpty(Vector2Int v)
    {
        return (v.x >= 0 && v.x <= 18 && v.y >= 0 && v.y <= 18 && !blackMoku.ContainsKey(v) && !whiteMoku.ContainsKey(v));
    }
}

class MokuGroup
{
    public Dictionary<Vector2Int, GameObject> mokuInGroup = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> matchingMoku;

    public MokuGroup(Dictionary<Vector2Int, GameObject> matchingMoku)
    {
        this.matchingMoku = matchingMoku;
        // Little hacky, but how else do we pick a starting point?
        GameObject start = null;
        foreach (GameObject m in matchingMoku.Values)
        {
            start = m;
            break;
        }
        RecursiveGrow(start);
    }

    public MokuGroup(Dictionary<Vector2Int, GameObject> matchingMoku, Vector2Int startingPoint)
    {
        this.matchingMoku = matchingMoku;
        RecursiveGrow(matchingMoku[startingPoint]);
    }

    public void RecursiveGrow(GameObject firstMoku)
    {
        if (firstMoku == null) return;
        mokuInGroup.Add(MokuPos(firstMoku), firstMoku);
        matchingMoku.Remove(MokuPos(firstMoku));

        RecursiveGrow(FindMatchingMoku(MokuPos(firstMoku) + Vector2Int.up));
        RecursiveGrow(FindMatchingMoku(MokuPos(firstMoku) + Vector2Int.down));
        RecursiveGrow(FindMatchingMoku(MokuPos(firstMoku) + Vector2Int.left));
        RecursiveGrow(FindMatchingMoku(MokuPos(firstMoku) + Vector2Int.right));
    }

    GameObject FindMatchingMoku(Vector2Int location)
    {
        if (matchingMoku.ContainsKey(location)) return matchingMoku[location];
        return null;
    }

    public static Vector2Int MokuPos(GameObject m)
    {
        return new Vector2Int(Mathf.RoundToInt(m.transform.position.x), Mathf.RoundToInt(m.transform.position.y));
    }
}
