using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour {

    public GameObject blackMoku, whiteMoku;

    public bool isBlacksTurn = true;

    public GameObject[,] stones = new GameObject[19, 19];

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
        // TODO: Fill out this function.
        // Any stones that are now surrounded because of the placement of the new stone
        // should be removed.
        
        // You do NOT need to worry about scoring - just remove any stones that are captured.
    }
}
