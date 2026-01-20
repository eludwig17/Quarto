
using UnityEngine;

public class QuartoGenerator : MonoBehaviour
{
    public Material whiteMaterial;
    public Material blackMaterial;

    void Start()
    {
        //CreateCylinder(new Vector(#, #, #), tall, hollow, white)
        CreateCylinder(new Vector3(-48, 1, 42), true, true, true);  
        CreateCylinder(new Vector3(-47, 1, 42), true, true, false);  
        CreateCylinder(new Vector3(-46, 1, 42), true, false, true); 
        CreateCylinder(new Vector3(-45, 1, 42), true, false, false);  

        CreateCylinder(new Vector3(-48, (float)0.5, 46), false, true, true); 
        CreateCylinder(new Vector3(-47, (float)0.5, 46), false, true, false);
        CreateCylinder(new Vector3(-46, (float)0.5, 46), false, false, true); 
        CreateCylinder(new Vector3(-45, (float)0.5, 46), false, false, false); 


        CreatePyramid(new Vector3(-48, 1, 44), true, true, true);   
        CreatePyramid(new Vector3(-47, 1, 44), true, false, true);  
        CreatePyramid(new Vector3(-46, 1, 44), true, true, false);   
        CreatePyramid(new Vector3(-45, 1, 44), true, false, false); 

        CreatePyramid(new Vector3(-48, (float)0.5, 48), false, true, true); 
        CreatePyramid(new Vector3(-47, (float)0.5, 48), false, false, true); 
        CreatePyramid(new Vector3(-46, (float)0.5, 48), false, true, false);  
        CreatePyramid(new Vector3(-45, (float)0.5, 48), false, false, false); 
    }

    void CreateCylinder(Vector3 position, bool tall, bool hollow, bool isWhite)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;

        cylinder.transform.localScale = tall ? new Vector3(1f, 1.2f, 1f) : new Vector3(1f, 0.6f, 1f);

        EnsureMaterialsExists();
        Material outerMaterial = isWhite ? whiteMaterial : blackMaterial;
        cylinder.GetComponent<Renderer>().material = outerMaterial;

        if (hollow)
            MakeHollow(cylinder, outerMaterial);
    }

    Mesh CreatePyramidMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0f, 1f, 0f)

        };

        int[] triangles = new int[]
        {
            0, 1, 4,
            1, 2, 4,
            2, 3, 4,
            3, 0, 4,
            3, 2, 1,
            3, 1, 0

        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    void CreatePyramid(Vector3 position, bool tall, bool hollow, bool isWhite)
    {
        GameObject pyramid = new GameObject("Pyramid");
        pyramid.transform.position = position;
        
        MeshFilter meshFilter = pyramid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pyramid.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = CreatePyramidMesh();

        pyramid.transform.localScale = tall ? new Vector3(1f, 1.2f, 1f) : new Vector3(1f, 0.6f, 1f);
        
        EnsureMaterialsExists();
        
        Material outerMaterial = isWhite ? whiteMaterial : blackMaterial;
        meshRenderer.material = outerMaterial;
        
        if (hollow)
            MakeHollow(pyramid, outerMaterial);
    }

    void MakeHollowPyramid(GameObject outer, Material outerMaterial)
    {
        GameObject inner =  new GameObject("InnerPyramid");
        inner.transform.parent = outer.transform;
        inner.transform.localPosition = Vector3.zero;
        inner.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
        
        MeshFilter meshFilter = inner.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = inner.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = CreatePyramidMesh();
        
        Material innerMaterial = new Material(outerMaterial);
        SetMaterialTransparent(innerMaterial, 0.25f);
        meshRenderer.material = innerMaterial;
    }

    void MakeHollow(GameObject outer, Material outerMaterial)
    {
        GameObject inner = GameObject.CreatePrimitive(PrimitiveType.Cylinder); 
        inner.transform.parent = outer.transform;
        inner.transform.localPosition = Vector3.zero;
        inner.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);

        Material innerMaterial = new Material(outerMaterial);
        SetMaterialTransparent(innerMaterial, 0.25f);
        inner.GetComponent<Renderer>().material = innerMaterial;
    }
    
    void EnsureMaterialsExists()
    {
        if (whiteMaterial == null)
        {
            whiteMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            whiteMaterial.color = Color.white;
        }

        if (blackMaterial == null)
        {
            blackMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            blackMaterial.color = Color.black;
        }
    }

    void SetMaterialTransparent(Material material, float transparency)
    {
        Color c = material.color;
        c.a = transparency;
        material.color = c;
        
        // I used an online resource but this is the Standard shader transparency setup
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.renderQueue = 3000;
    }
}
