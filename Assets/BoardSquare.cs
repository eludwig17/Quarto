using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BoardSquare : MonoBehaviour {
    // columns & rows go from 0-3 based off game board positon
    public int row;
    public int col;

    [Header("Highlight Settings")]
    public Color hoverColor = Color.gray;

    private Renderer _renderer;
    private Color _originalColor;
    private bool _isHovered;

    private void Awake(){
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }

    public void SetHovered(bool hovered){
        if (_isHovered == hovered) return;
        _isHovered = hovered;
        
        if (_renderer == null) return;
        
        _renderer.material.color = _isHovered ? hoverColor : _originalColor;
    }
}