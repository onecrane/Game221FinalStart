using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour {

    public GameObject blackMoku, whiteMoku;

    public bool isBlacksTurn = true;

    public GameObject[,] stones = new GameObject[19, 19];
    private Vector2Int U = Vector2Int.up, D = Vector2Int.down, L = Vector2Int.left, R = Vector2Int.right;

	// Use this for initialization
	void Start () {

        GameObject[] existingStones = GameObject.FindGameObjectsWithTag("Moku");
        for (int i = 0; i < existingStones.Length; i++)
        {
            Vector3 existingStoneLocation = existingStones[i].transform.position;
            stones[Mathf.RoundToInt(existingStoneLocation.x), Mathf.RoundToInt(existingStoneLocation.y)] = existingStones[i];
        }

	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int bx = Mathf.RoundToInt(mousePosition.x), by = Mathf.RoundToInt(mousePosition.y);

            if (bx >= 0 && bx <= 18 && by >= 0 && by <= 18 && stones[bx, by] == null)
            {
                GameObject newMoku = GameObject.Instantiate(isBlacksTurn ? blackMoku : whiteMoku);
                newMoku.transform.position = new Vector3(bx, by, 0);

                stones[bx, by] = newMoku;

                RemoveCapturedStones();

                isBlacksTurn = !isBlacksTurn;
            }
        }
	}

    private void RemoveCapturedStones()
    {
        GameObject[] allMoku = GameObject.FindGameObjectsWithTag("Moku");

        List<GameObject> opponentMoku = new List<GameObject>();
        string opponentPrefix = isBlacksTurn ? "White" : "Black";
        foreach (GameObject moku in allMoku) if (moku.name.StartsWith(opponentPrefix)) opponentMoku.Add(moku);

        while (opponentMoku.Count > 0)
        {
            MokuGroup group = new MokuGroup(opponentMoku);

            bool groupCanBreathe = false;
            foreach (GameObject moku in group.mokuInGroup) if (MokuCanBreathe(moku)) groupCanBreathe = true;

            if (!groupCanBreathe)
                foreach (GameObject moku in group.mokuInGroup)
                {
                    Destroy(moku);
                    Vector2Int pos = new Vector2Int(Mathf.RoundToInt(moku.transform.position.x), Mathf.RoundToInt(moku.transform.position.y));
                    stones[pos.x, pos.y] = null;
                }
        }
    }

    bool MokuCanBreathe(GameObject moku)
    {
        return ValidAndEmpty(MokuGroup.MokuPos(moku) + U) || ValidAndEmpty(MokuGroup.MokuPos(moku) + D) || ValidAndEmpty(MokuGroup.MokuPos(moku) + L) || ValidAndEmpty(MokuGroup.MokuPos(moku) + R);
    }

    bool ValidAndEmpty(Vector2Int v)
    {
        return (v.x >= 0 && v.x <= 18 && v.y >= 0 && v.y <= 18 && stones[v.x, v.y] == null);
    }
}

class MokuGroup
{
    public List<GameObject> mokuInGroup = new List<GameObject>();
    private List<GameObject> matchingMoku;

    public MokuGroup(List<GameObject> matchingMoku)
    {
        this.matchingMoku = matchingMoku;
        RecursiveGrow(matchingMoku[0]);
    }

    public void RecursiveGrow(GameObject firstMoku)
    {
        if (firstMoku == null) return;
        mokuInGroup.Add(firstMoku);
        matchingMoku.Remove(firstMoku);

        RecursiveGrow(FindMatchingMoku(firstMoku, MokuPos(firstMoku) + Vector2Int.up));
        RecursiveGrow(FindMatchingMoku(firstMoku, MokuPos(firstMoku) + Vector2Int.down));
        RecursiveGrow(FindMatchingMoku(firstMoku, MokuPos(firstMoku) + Vector2Int.left));
        RecursiveGrow(FindMatchingMoku(firstMoku, MokuPos(firstMoku) + Vector2Int.right));
    }

    GameObject FindMatchingMoku(GameObject originalMoku, Vector2Int location)
    {
        foreach (GameObject searchedMoku in matchingMoku) if (MokuPos(searchedMoku) == location) return searchedMoku;
        return null;
    }

    public static Vector2Int MokuPos(GameObject m)
    {
        return new Vector2Int(Mathf.RoundToInt(m.transform.position.x), Mathf.RoundToInt(m.transform.position.y));
    }
}
