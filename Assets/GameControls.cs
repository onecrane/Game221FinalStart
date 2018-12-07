using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour {

    public GameObject blackMoku, whiteMoku;

    public GameObject topLeft, bottomRight;

    public bool isBlacksTurn = true;

    private Vector2 cellSize;
    private float minX, maxX, minY, maxY;

	// Use this for initialization
	void Start () {
        cellSize = new Vector2((bottomRight.transform.position.x - topLeft.transform.position.x) / 18, (bottomRight.transform.position.y - topLeft.transform.position.y) / 18);
        minX = topLeft.transform.position.x;
        maxX = bottomRight.transform.position.x;
        minY = bottomRight.transform.position.y;
        maxY = topLeft.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mx = mousePosition.x, my = mousePosition.y;

            if (mx >= minX - cellSize.x * 0.4f  && mx <= maxX + cellSize.x * 0.4f && my >= minY - cellSize.y * 0.4f && my <= maxY + cellSize.y * 0.4f)
            {
                // Valid move, find closest cell
                int xCell = Mathf.RoundToInt((mx - minX) / cellSize.x);
                int yCell = Mathf.RoundToInt((my - minY) / cellSize.y);

                Vector2 location = new Vector2(xCell * cellSize.x + minX, yCell * cellSize.y + minY);

                GameObject moku = isBlacksTurn ? blackMoku : whiteMoku;

                GameObject newMoku = GameObject.Instantiate(moku);
                newMoku.transform.position = new Vector3(location.x, location.y, newMoku.transform.position.z);

                isBlacksTurn = !isBlacksTurn;
            }

        }

	}
}
