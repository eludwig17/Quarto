using System.Collections.Generic;
using UnityEngine;

public class QuartoPieceIndexRegistry : MonoBehaviour {
    readonly Dictionary<string, byte> _byName = new Dictionary<string, byte>();
    readonly GameObject[] _byIndex = new GameObject[16];
    bool _built;

    void EnsureBuilt(){
        if (_built) return;
        _built = true;
        _byName.Clear();
        for (int i = 0; i < _byIndex.Length; i++)
            _byIndex[i] = null;

        var pieces = GameObject.FindGameObjectsWithTag("GamePiece");
        foreach (var p in pieces){
            if (QuartoPieceCatalog.TryGetIndex(p.name, out byte idx)){
                _byName[p.name] = idx;
                _byIndex[idx] = p;
            }
        }
    }

    public bool TryGetIndex(GameObject piece, out byte index){
        EnsureBuilt();
        index = 0;
        if (piece == null) return false;
        return _byName.TryGetValue(piece.name, out index);
    }

    public GameObject GetPiece(byte index){
        EnsureBuilt();
        if (index > 15) return null;
        return _byIndex[index];
    }
}