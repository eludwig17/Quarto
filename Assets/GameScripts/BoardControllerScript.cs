using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class BoardControllerScript : MonoBehaviour
{
    Camera cam; // from Unity official docs
    //private Dictionary<string, GameObject> squareDict; // all squareDict code was useful for referencing squares by name
    
    public GameObject pieceSelected;
    private GameObject squareSelected;

    private GameObject hoveredPiece;
    private GameObject hoveredSquare;

    // [Alteruna.Synchronizable]
    public GameState gameState;
    // make sure to map for networked gameplayer & it maps the gameObjects to indices
    public QuartoPieceIndexRegistry pieceIndexRegistry;
    public string PlayerTurn;
    
    [SerializeField] public UnityEvent<string> OnTurnPlayed = new UnityEvent<string>();

    void Awake()
    {
        // squareDict = new Dictionary<string, GameObject>();
        cam = Camera.main;

        pieceSelected = null;
        squareSelected = null;
        hoveredPiece = null;
        hoveredSquare = null;
        
        PlayerTurn = "Player 1";

    }

    void Start(){
        if (gameState == null)
            gameState = GameState.Instance;
        if (pieceIndexRegistry == null)
            pieceIndexRegistry = GetComponentInChildren<QuartoPieceIndexRegistry>();
    }

    bool UseNetworkPlay() => gameState != null && gameState.CanPlay;

    bool BlockInputNotMyTurn() => gameState != null && gameState.IsInRoom && (!gameState.CanPlay || !gameState.IsLocalPlayersTurn);
    

    void Update()
    {
        UpdateHoverHighlights();
        if (gameState != null && gameState.IsInRoom)
            PlayerTurn = gameState.CanPlay
                ? (gameState.IsLocalPlayersTurn ? "Your turn" : "Opponent's turn")
                : "Waiting for opponent";
        playTurn();

    }

    void UpdateHoverHighlights()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit);

        GameObject newHoveredPiece = null;
        GameObject newHoveredSquare = null;

        if (didHit && hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("GamePiece"))
            {
                newHoveredPiece = hitObject;
            }
            else if (hitObject.CompareTag("BoardSquare"))
            {
                newHoveredSquare = hitObject;
            }
        }

        // Update piece hover highlight
        if (hoveredPiece != newHoveredPiece)
        {
            if (hoveredPiece != null)
            {
                PieceHighlight oldHighlight = hoveredPiece.GetComponent<PieceHighlight>();
                if (oldHighlight != null && hoveredPiece != pieceSelected)
                {
                    oldHighlight.SetHovered(false);
                }
            }

            hoveredPiece = newHoveredPiece;

            if (hoveredPiece != null && hoveredPiece != pieceSelected)
            {
                PieceHighlight newHighlight = hoveredPiece.GetComponent<PieceHighlight>();
                if (newHighlight != null)
                {
                    newHighlight.SetHovered(true);
                }
            }
        }

        // Update square hover highlight
        if (hoveredSquare != newHoveredSquare)
        {
            if (hoveredSquare != null)
            {
                BoardSquare oldSquare = hoveredSquare.GetComponent<BoardSquare>();
                if (oldSquare != null && hoveredSquare != squareSelected)
                {
                    oldSquare.SetHovered(false);
                }
            }

            hoveredSquare = newHoveredSquare;

            if (hoveredSquare != null && hoveredSquare != squareSelected)
            {
                BoardSquare newSquare = hoveredSquare.GetComponent<BoardSquare>();
                if (newSquare != null)
                {
                    newSquare.SetHovered(true);
                }
            }
        }
    }

    void playTurn()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (BlockInputNotMyTurn())
                return;
            if (!pieceSelected)
            {
                GameObject temp = getObjectClickedOn();
                if (temp != null)
                {

                    if (temp.tag.Contains("GamePiece"))
                    {
                        pieceSelected = temp;
                        //Debug.Log($"Selecting piece {pieceSelected.name}");

                        // selected game piece will stay highlighted even if not hovered upon
                        PieceHighlight pieceHighlight = pieceSelected.GetComponent<PieceHighlight>();
                        if (pieceHighlight != null)
                        {
                            pieceHighlight.SetSelected(true);
                        }
                        // clears the hover state
                        if (hoveredPiece == pieceSelected)
                        {
                            hoveredPiece = null;
                        }
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
                        //Debug.Log($"Selecting square {squareSelected.name}");

                        bool placementSucceeded = false;

                        if (UseNetworkPlay()){
                            if (pieceIndexRegistry != null && pieceIndexRegistry.TryGetIndex(pieceSelected, out byte pIdx)){
                                BoardSquare bs = squareSelected.GetComponent<BoardSquare>();
                                if (bs != null){
                                    gameState.RequestPlacePiece(pIdx, bs.row, bs.col);
                                    placementSucceeded = true;
                                    OnTurnPlayed?.Invoke("Waiting…");
                                }
                            }
                            else{
                                Debug.LogWarning("This game piece isn't in the registry");
                            }
                        }
                        else if (gameState != null)
                        {
                            placementSucceeded = gameState.TryPlacePiece(pieceSelected, squareSelected);
                        }

                        if (placementSucceeded)
                        {
                            PlayerTurn = (PlayerTurn == "Player 1") ? "Player 2" : "Player 1";
                            placePiece(pieceSelected, squareSelected);
                            Debug.Log("INVOKEEEEE");
                            OnTurnPlayed?.Invoke(PlayerTurn);
                        }
                        else
                        {
                            Debug.Log("You can't place a piece here... spots already taken");
                        }

                        if (pieceSelected != null)
                        {
                            PieceHighlight pieceHighlight = pieceSelected.GetComponent<PieceHighlight>();
                            if (pieceHighlight != null)
                            {
                                pieceHighlight.SetSelected(false);
                                pieceHighlight.SetHovered(false);
                            }
                        }
                        if (squareSelected != null)
                        {
                            BoardSquare selectedSquare = squareSelected.GetComponent<BoardSquare>();
                            if (selectedSquare != null)
                            {
                                selectedSquare.SetHovered(false);
                            }
                        }

                        if (hoveredPiece == pieceSelected)
                        {
                            hoveredPiece = null;
                        }
                        if (hoveredSquare == squareSelected)
                        {
                            hoveredSquare = null;
                        }

                        pieceSelected = null;
                        squareSelected = null;
                    }
            }
        }


    }

    public void ApplyNetworkMove(GameObject piece, GameObject square){
        placePiece(piece, square);
    }

    public void OnNetworkTurnChanged(int currentTurnPlayerIndex){
        bool mine = gameState != null && gameState.IsLocalPlayersTurn;
        PlayerTurn = mine ? "Your turn" : "Opponent's turn";
        OnTurnPlayed?.Invoke(PlayerTurn);
    }

    void placePiece(GameObject piece, GameObject square)
    {
        //GameObject square = squareDict[squareName];
        float y_size_square = square.GetComponent<Renderer>().bounds.size.y;
        float y_size_piece = piece.GetComponent<Renderer>().bounds.size.y;

        float y_pos = square.transform.position.y + (y_size_square + y_size_piece) / 2;

        piece.transform.position = new Vector3(square.transform.position.x, y_pos, square.transform.position.z);

        Collider pieceCollider = piece.GetComponent<Collider>();
        if (pieceCollider != null)
        {
            pieceCollider.enabled = false;
        }
    }

    GameObject getObjectClickedOn()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        bool did_hit = Physics.Raycast(ray, out RaycastHit endHit);

        if (did_hit)    // Physics.Raycast returns false if it did not hit an object
        {
            GameObject hitObject = endHit.collider.gameObject;
            return hitObject;

        }
        else
        {
            return null;
        }
    }
}
