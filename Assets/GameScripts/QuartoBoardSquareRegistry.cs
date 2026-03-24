using UnityEngine;

public class QuartoBoardSquareRegistry : MonoBehaviour {
    BoardSquare[,] _cells;

    void Awake(){
        _cells = new BoardSquare[4, 4];
        var squares = FindObjectsByType<BoardSquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var bs in squares){
            if (bs.row >= 0 && bs.row < 4 && bs.col >= 0 && bs.col < 4)
                _cells[bs.row, bs.col] = bs;
        }
    }

    public BoardSquare Get(int row, int col){
        if (row < 0 || row > 3 || col < 0 || col > 3) return null;
        return _cells[row, col];
    }
}