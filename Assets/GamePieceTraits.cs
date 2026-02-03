using UnityEngine;

[DisallowMultipleComponent]
public class GamePieceTraits : MonoBehaviour {
    /*Bit layout (LSB -> MSB)
    0 | tall is 1, short is 0
    1 | prism is 1, cylinder is 0
    2 | hollow is 1, solid is 0
    3 | white is 1, black is 0*/
    public const int TallBit = 1 << 0;
    public const int PrismBit = 1 << 1;
    public const int HollowBit = 1 << 2;
    public const int WhiteBit = 1 << 3;
    public const int AllBits = TallBit | PrismBit | HollowBit | WhiteBit;

    [SerializeField] private bool _isTall;
    [SerializeField] private bool _isPrism;
    [SerializeField] private bool _isHollow;
    [SerializeField] private bool _isWhite;

    public bool IsTall => _isTall;
    public bool IsPrism => _isPrism;
    public bool IsHollow => _isHollow;
    public bool IsWhite => _isWhite;

    public int TraitMask{
        get{
            int mask = 0;
            if (_isTall) mask |= TallBit;
            if (_isPrism) mask |= PrismBit;
            if (_isHollow) mask |= HollowBit;
            if (_isWhite) mask |= WhiteBit;
            return mask;
        }
    }
    
    public void SetTraits(bool isTall, bool isPrism, bool isHollow, bool isWhite){
        _isTall = isTall;
        _isPrism = isPrism;
        _isHollow = isHollow;
        _isWhite = isWhite;
    }
}