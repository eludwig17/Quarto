using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    Camera cam; // from Unity official docs
    //private Dictionary<string, GameObject> squareDict; // all squareDict code was useful for referencing squares by name
    private GameObject pieceSelected;
    private GameObject squareSelected;

    void Start()
    {
        // squareDict = new Dictionary<string, GameObject>();
        cam = Camera.main;

        pieceSelected = null;
        squareSelected = null;

        //GameObject[] allSquares = GameObject.FindGameObjectsWithTag("BoardSquare"); // this may be able to just be hardcoded in

        // Takes all board squares and adds them to a dict of their name and corresponding object
        // this allows them to be referenced by their name in placePiece
        // foreach (GameObject obj in allSquares)
        // {
        //     squareDict.Add(obj.name, obj);
        //     print($"adding object {obj.name} to dict");
        // }
    }

    void Update()
    {

        playTurn();

    }

    void playTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!pieceSelected)
            {
                GameObject temp = getObjectClickedOn();
                if (temp != null)
                {

                    if (temp.tag.Contains("GamePiece"))
                    {
                        pieceSelected = temp;
                        Debug.Log($"Selecting piece {pieceSelected.name}");
                    }
                }
            }
            else if (!squareSelected)
            {

                GameObject temp = getObjectClickedOn();
                if (temp != null)
                    if (temp.tag.Contains("BoardSquare")) // verifies that the thing clicked is a square
                    {
                        squareSelected = temp;
                        Debug.Log($"Selecting square {squareSelected.name}");

                        placePiece(pieceSelected, squareSelected);

                        pieceSelected = null;
                        squareSelected = null;
                    }

            }
        }
    }

    void placePiece(GameObject piece, GameObject square)
    {
        //GameObject square = squareDict[squareName];
        float y_size_square = square.GetComponent<Renderer>().bounds.size.y;
        float y_size_piece = piece.GetComponent<Renderer>().bounds.size.y;

        float y_pos = square.transform.position.y + (y_size_square + y_size_piece) / 2;

        piece.transform.position = new Vector3(square.transform.position.x, y_pos, square.transform.position.z);
    }

    GameObject getObjectClickedOn()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        bool did_hit = Physics.Raycast(ray, out RaycastHit endHit);

        // print(hit.collider.gameObject.name);

        if (did_hit)    // Phsyics.Raycast returns false if it did not hit an object
        {
            GameObject hitObject = endHit.collider.gameObject;
            return hitObject;

        } else
        {
            return null;
        }

    }
}
