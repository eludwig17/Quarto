using System.Collections.Generic;
using System.Linq;
using Alteruna;
using UnityEngine;

public class GameState : AttributesSync
{
    public static GameState Instance { get; private set; }
    
    private enum Shape
    {
        Cylinder,
        Prism
    }

    private enum Hollowness
    {
        Hollow,
        Solid
    }

    private enum Height
    {
        Tall,
        Short
    }

    private enum Color
    {
        White,
        Black
    }

    public List<GameObject> unplacedPieces = new List<GameObject>();
    public GameObject[,] boardPieces = new GameObject[4, 4];
    [SynchronizableField] private int currentTurnPlayerIndex = 0;

    public bool IsInRoom{
        get { return Multiplayer != null && Multiplayer.InRoom; }
    }

    public int UserCount{
        get{
            if (Multiplayer == null || Multiplayer.CurrentRoom == null || Multiplayer.CurrentRoom.Users == null){
                return 0;
            }

            return Multiplayer.CurrentRoom.Users.Count;
        }
    }

    public bool CanPlay{
        get { return IsInRoom && UserCount >= 2; }
    }

    public int GetGamePlayerIndex(ushort alterunaUserIndex){
        if (Multiplayer == null || Multiplayer.CurrentRoom == null || Multiplayer.CurrentRoom.Users == null || Multiplayer.CurrentRoom.Users.Count < 2){
            return -1;
        }

        var sorted = Multiplayer.CurrentRoom.Users.OrderBy(u => u.Index).ToList();
        for (int i = 0; i < sorted.Count; i++){
            if (sorted[i].Index == alterunaUserIndex){
                return i;
            }
        }

        return -1;
    }

    public bool IsLocalPlayersTurn{
        get{
            if (!CanPlay || Multiplayer == null || Multiplayer.Me == null) return false;
            int myPlayer = GetGamePlayerIndex((ushort)Multiplayer.Me.Index);
            return myPlayer >= 0 && myPlayer == currentTurnPlayerIndex;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (Multiplayer == null){
            Multiplayer = FindFirstObjectByType<Multiplayer>();
        }
        GameObject[] piecesInScene = GameObject.FindGameObjectsWithTag("GamePiece");
        unplacedPieces.AddRange(piecesInScene);
    }

    public void RequestPlacePiece(int pieceIndex, int row, int col){
        if (!CanPlay || !IsLocalPlayersTurn || Multiplayer == null || Multiplayer.Me == null) return;
        BroadcastRemoteMethod(nameof(PlacePieceNetwork), (ushort)Multiplayer.Me.Index, pieceIndex, row, col);
    }

    [SynchronizableMethod]
    private void PlacePieceNetwork(ushort fromUserIndex, int pieceIndex, int row, int col){
        if (pieceIndex < 0 || pieceIndex > 15 || row < 0 || row > 3 || col < 0 || col > 3) return;

        if (!CanPlay) return;

        int fromPlayer = GetGamePlayerIndex(fromUserIndex);
        if (fromPlayer < 0 || fromPlayer != currentTurnPlayerIndex) return;

        QuartoPieceIndexRegistry pieceReg = FindFirstObjectByType<QuartoPieceIndexRegistry>();
        QuartoBoardSquareRegistry squareReg = FindFirstObjectByType<QuartoBoardSquareRegistry>();
        BoardControllerScript board = FindFirstObjectByType<BoardControllerScript>();

        if (pieceReg == null || squareReg == null || board == null) return;

        GameObject piece = pieceReg.GetPiece((byte)pieceIndex);
        BoardSquare square = squareReg.Get(row, col);
        if (piece == null || square == null) return;

        if (boardPieces[row, col] != null) return;
        if (!unplacedPieces.Contains(piece)) return;

        boardPieces[row, col] = piece;
        unplacedPieces.Remove(piece);

        currentTurnPlayerIndex = 1 - currentTurnPlayerIndex;
        Commit();

        board.ApplyNetworkMove(piece, square.gameObject);
        board.OnNetworkTurnChanged(currentTurnPlayerIndex);
    }
    
    public bool TryPlacePiece(GameObject piece, GameObject square)
    {
        if (piece == null || square == null) return false;

        // gets the board pos
        BoardSquare squareComponent = square.GetComponent<BoardSquare>();
        if (squareComponent == null) return false;

        int row = squareComponent.row;
        int col = squareComponent.col;

        if (row < 0 || row > 3 || col < 0 || col > 3) return false; // here checks if board positon/valid exists for both row/col
        if (boardPieces[row, col] != null) return false; // checks board spots
        if (!unplacedPieces.Contains(piece)) return false; // checks if available to place

        // places piece & updates state
        boardPieces[row, col] = piece;
        unplacedPieces.Remove(piece);

        return true;
    }

    public bool CheckRowForWin(int row)
    {
        if (row < 0 || row > 3) return false;
        
        return CheckLineForWin(
            boardPieces[0, row],
            boardPieces[1, row],
            boardPieces[2, row],
            boardPieces[3, row]
        );
    }

    public bool CheckColumnForWin(int column)
    {
        if (column < 0 || column > 3) return false;
        
        return CheckLineForWin(
            boardPieces[0, column],
            boardPieces[1, column],
            boardPieces[2, column],
            boardPieces[3, column]
            );
    }

    public bool CheckDiagonalForWin(int diagonal)
    {
        if (diagonal == 0)
        {
            return CheckLineForWin(
                boardPieces[0, 0],
                boardPieces[1, 1],
                boardPieces[2, 2],
                boardPieces[3, 3]
            );
        }
        else if (diagonal == 1)
        {
            return CheckLineForWin(
                boardPieces[0, 3],
                boardPieces[1, 2],
                boardPieces[2, 1],
                boardPieces[3, 0]
            );
        }

        return false;

    }

    private bool CheckLineForWin(GameObject a, GameObject b, GameObject c, GameObject d)
    {
        if (a == null || b == null || c == null || d == null) return false;
        
        GetFullShapeInfo(a.name, out Height firstHeight, out Hollowness firstHollowness, out Color firstColor, 
            out Shape firstShape);

        bool sameColor = true;
        bool sameHeight = true;
        bool sameShape = true;
        bool sameHollowness = true;
        
        GameObject[] line = {b, c, d};

        foreach (GameObject piece in line)
        {
            GetFullShapeInfo(piece.name, out Height pieceHeight, out Hollowness pieceHollow, out Color pieceColor,
                out Shape pieceShape);
            if (pieceColor != firstColor) sameColor = false;
            if (pieceHeight != firstHeight) sameHeight = false;
            if (pieceShape != firstShape) sameShape = false;
            if (pieceHollow != firstHollowness) sameHollowness = false;
        }
        return sameColor || sameHeight || sameShape || sameHollowness;
    }

    // some code in this function is from ChatGPT, idea from Geist
    string GetShape(string name)
    {

        int start = -1;

        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (char.IsUpper(name[i]))
            {
                start = i;
                break;
            }
        }

        string result = start >= 0 ? name.Substring(start) : string.Empty;

        return result;

    }

    // naming scheme: TallHollowWhiteCylinder
    //                HeightHollownessColorShape

    // Outputs the enum type of each category for the given shape
    void GetFullShapeInfo(string name, out Height height, out Hollowness hollow, out Color material, out Shape shape)
    {
        
        height = name.Contains("Tall") ? Height.Tall : Height.Short;
        hollow = name.Contains("Hollow") ? Hollowness.Hollow : Hollowness.Solid;
        material = name.Contains("White") ? Color.White : Color.Black;
        shape = name.Contains("Cylinder") ? Shape.Cylinder : Shape.Prism;

    }

}