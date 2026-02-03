using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    Camera cam; // from Unity official docs
    //private Dictionary<string, GameObject> squareDict; // all squareDict code was useful for referencing squares by name
    private GameObject pieceSelected;
    private GameObject squareSelected;

    private GameObject hoveredPiece;
    private GameObject hoveredSquare;

    private GameState gameState;

    void Start()
    {
        // squareDict = new Dictionary<string, GameObject>();
        cam = Camera.main;

        pieceSelected = null;
        squareSelected = null;
        hoveredPiece = null;
        hoveredSquare = null;

        gameState = FindObjectOfType<GameState>();

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
        UpdateHoverHighlights();
        playTurn();

    }
    
    void UpdateHoverHighlights(){

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit);

        GameObject newHoveredPiece = null;
        GameObject newHoveredSquare = null;

        if (didHit && hit.collider != null){
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("GamePiece")){
                newHoveredPiece = hitObject;
            }
            else if (hitObject.CompareTag("BoardSquare")){
                newHoveredSquare = hitObject;
            }
        }

        // Update piece hover highlight
        if (hoveredPiece != newHoveredPiece)
        {
            if (hoveredPiece != null)
            {
                PieceHighlight oldHighlight = hoveredPiece.GetComponent<PieceHighlight>();
                if (oldHighlight != null && hoveredPiece != pieceSelected){
                    oldHighlight.SetHovered(false);
                }
            }

            hoveredPiece = newHoveredPiece;

            if (hoveredPiece != null && hoveredPiece != pieceSelected){
                PieceHighlight newHighlight = hoveredPiece.GetComponent<PieceHighlight>();
                if (newHighlight != null){
                    newHighlight.SetHovered(true);
                }
            }
        }

        // Update square hover highlight
        if (hoveredSquare != newHoveredSquare){
            if (hoveredSquare != null){
                BoardSquare oldSquare = hoveredSquare.GetComponent<BoardSquare>();
                if (oldSquare != null && hoveredSquare != squareSelected){
                    oldSquare.SetHovered(false);
                }
            }

            hoveredSquare = newHoveredSquare;

            if (hoveredSquare != null && hoveredSquare != squareSelected){
                BoardSquare newSquare = hoveredSquare.GetComponent<BoardSquare>();
                if (newSquare != null){
                    newSquare.SetHovered(true);
                }
            }
        }
    }

    void playTurn(){
        if (Input.GetMouseButtonDown(0)){
            if (!pieceSelected){
                GameObject temp = getObjectClickedOn();
                if (temp != null){

                    if (temp.tag.Contains("GamePiece")){
                        pieceSelected = temp;
                        Debug.Log($"Selecting piece {pieceSelected.name}");

                        // selected game piece will stay highlighted even if not hovered upon
                        PieceHighlight pieceHighlight = pieceSelected.GetComponent<PieceHighlight>();
                        if (pieceHighlight != null){
                            pieceHighlight.SetSelected(true);
                        }
                        // clears the hover state
                        if (hoveredPiece == pieceSelected){
                            hoveredPiece = null;
                        }
                    }
                }
            }
            else if (!squareSelected){

                GameObject temp = getObjectClickedOn();
                if (temp != null)
                    if (temp.tag.Contains("BoardSquare")) // verifies that the thing clicked is a square
                    {
                        squareSelected = temp;
                        Debug.Log($"Selecting square {squareSelected.name}");

                        bool placementSucceeded = true;
                        if (gameState != null){
                            placementSucceeded = gameState.TryPlacePiece(pieceSelected, squareSelected);
                        }

                        if (placementSucceeded){
                            placePiece(pieceSelected, squareSelected);
                            for (int row = 0; row < 4; row++){
                                if (gameState != null && gameState.IsWinningRow(row)){
                                    break;
                                }
                            }
                        }
                        else{
                            Debug.Log("You can't place a piece here... spots already taken");
                        }

                        if (pieceSelected != null){
                            PieceHighlight pieceHighlight = pieceSelected.GetComponent<PieceHighlight>();
                            if (pieceHighlight != null){
                                pieceHighlight.SetSelected(false);
                                pieceHighlight.SetHovered(false);
                            }
                        }
                        if (squareSelected != null){
                            BoardSquare selectedSquare = squareSelected.GetComponent<BoardSquare>();
                            if (selectedSquare != null){
                                selectedSquare.SetHovered(false);
                            }
                        }

                        if (hoveredPiece == pieceSelected){
                            hoveredPiece = null;
                        }
                        if (hoveredSquare == squareSelected){
                            hoveredSquare = null;
                        }

                        pieceSelected = null;
                        squareSelected = null;
                    }
            }
        }
    }

    void placePiece(GameObject piece, GameObject square){
        //GameObject square = squareDict[squareName];
        float y_size_square = square.GetComponent<Renderer>().bounds.size.y;
        float y_size_piece = piece.GetComponent<Renderer>().bounds.size.y;

        float y_pos = square.transform.position.y + (y_size_square + y_size_piece) / 2;

        piece.transform.position = new Vector3(square.transform.position.x, y_pos, square.transform.position.z);
        
        Collider pieceCollider = piece.GetComponent<Collider>();
        if (pieceCollider != null){
            pieceCollider.enabled = false;
        }
    }

    GameObject getObjectClickedOn(){

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        bool did_hit = Physics.Raycast(ray, out RaycastHit endHit);

        // print(hit.collider.gameObject.name);

        if (did_hit)    // Physics.Raycast returns false if it did not hit an object
        {
            GameObject hitObject = endHit.collider.gameObject;
            return hitObject;

        }
        else{
            return null;
        }
    }
}
