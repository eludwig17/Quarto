using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PieceHighlight : MonoBehaviour {
    [Header("Highlight Colors")]
    public Color hoverColor = Color.lightBlue;
    public Color selectedColor = Color.darkBlue;
    
    [Header("Outline Settings")]
    public float outlineScale = 1.1f;

    private bool _isHovered;
    private bool _isSelected;
    private GameObject _outlineObject;

    public void SetHovered(bool hovered){
        if (_isHovered == hovered) return;
        _isHovered = hovered;
        ApplyOutline();
    }

    public void SetSelected(bool selected){
        if (_isSelected == selected) return;
        _isSelected = selected;
        ApplyOutline();
    }

    private void ApplyOutline(){
        if (_isSelected){
            CreateOutline(selectedColor);
        }
        else if (_isHovered){
            CreateOutline(hoverColor);
        }
        else{
            RemoveOutline();
        }
    }

    private void CreateOutline(Color color){
        if (_outlineObject == null){
            MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
            if (originalMeshFilter == null) return;

            _outlineObject = new GameObject(gameObject.name + "_Outline");
            _outlineObject.transform.SetParent(transform);
            _outlineObject.transform.localPosition = Vector3.zero;
            _outlineObject.transform.localRotation = Quaternion.identity;
            _outlineObject.transform.localScale = Vector3.one * outlineScale;
            
            MeshFilter meshFilter = _outlineObject.AddComponent<MeshFilter>();
            meshFilter.mesh = originalMeshFilter.mesh;
            
            MeshRenderer outlineRenderer = _outlineObject.AddComponent<MeshRenderer>();
            
            Material outlineMat = new Material(Shader.Find("Sprites/Default"));
            if (outlineMat.shader == null || outlineMat.shader.name == "Hidden/InternalErrorShader"){
                outlineMat = new Material(Shader.Find("UI/Default"));
            }
            
            outlineMat.color = color;
            outlineRenderer.material = outlineMat;
            outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineRenderer.receiveShadows = false;
        }
        else{
            _outlineObject.GetComponent<Renderer>().material.color = color;
        }
    }

    private void RemoveOutline(){
        if (_outlineObject != null){
            Destroy(_outlineObject);
            _outlineObject = null;
        }
    }

    private void OnDestroy(){
        RemoveOutline();
    }
}