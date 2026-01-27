
using UnityEngine;

public class GamePieces : MonoBehaviour{
    public Material whiteMaterial;
    public Material blackMaterial;
    public Material whiteMaterialHollow;
    public Material blackMaterialHollow;

    void Start(){
        // short cylinders
        CreateCylinder(new Vector3(-48, 1, 42), true, true, true);
        CreateCylinder(new Vector3(-46, 1, 42), true, true, false);
        CreateCylinder(new Vector3(-44, 1, 42), true, false, true);
        CreateCylinder(new Vector3(-42, 1, 42), true, false, false);
        // short cylinders
        CreateCylinder(new Vector3(-48, 0.5f, 46), false, true, true);
        CreateCylinder(new Vector3(-46, 0.5f, 46), false, true, false);
        CreateCylinder(new Vector3(-44, 0.5f, 46), false, false, true);
        CreateCylinder(new Vector3(-42, 0.5f, 46), false, false, false);
        // tall prisms
        CreateTriangularPrism(new Vector3(-48, 1, 44), true, true, true);
        CreateTriangularPrism(new Vector3(-46, 1, 44), true, true, false);
        CreateTriangularPrism(new Vector3(-44, 1, 44), true, false, true);
        CreateTriangularPrism(new Vector3(-42, 1, 44), true, false, false);
        // short prisms
        CreateTriangularPrism(new Vector3(-48, 0.5f, 48), false, true, true);
        CreateTriangularPrism(new Vector3(-46, 0.5f, 48), false, true, false);
        CreateTriangularPrism(new Vector3(-44, 0.5f, 48), false, false, true);
        CreateTriangularPrism(new Vector3(-42, 0.5f, 48), false, false, false);
    }

    void CreateCylinder(Vector3 position, bool tall, bool hollow, bool isWhite){
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
        cylinder.transform.localScale = tall ? new Vector3(1f, 1.2f, 1f) : new Vector3(1f, 0.6f, 1f);

        Material material;
        if (hollow){
            material = isWhite ? whiteMaterialHollow : blackMaterialHollow;
            MakeHollow(cylinder);
        }
        else{
            material = isWhite ? whiteMaterial : blackMaterial;
        }

        cylinder.GetComponent<Renderer>().material = material;
    }

    void MakeHollow(GameObject outer){
        MeshFilter meshFilter = outer.GetComponent<MeshFilter>();
        meshFilter.mesh = CreateHollowCylinderMesh(0.5f, 0.35f, 2f, 32);
    }

    Mesh CreateHollowCylinderMesh(float outerRadius, float innerRadius, float height, int segments){
        Mesh mesh = new Mesh();
        int vertexCount = segments * 8;
        int triangleCount = segments * 8;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount * 3];
        float angleStep = 2f * Mathf.PI / segments;

        for (int i = 0; i < segments; i++){
            float angle = i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            int baseIndex = i * 8;

            vertices[baseIndex + 0] = new Vector3(cos * outerRadius, -height / 2, sin * outerRadius);
            vertices[baseIndex + 1] = new Vector3(cos * outerRadius, height / 2, sin * outerRadius);
            vertices[baseIndex + 2] = new Vector3(cos * innerRadius, -height / 2, sin * innerRadius);
            vertices[baseIndex + 3] = new Vector3(cos * innerRadius, height / 2, sin * innerRadius);

            vertices[baseIndex + 4] = new Vector3(cos * outerRadius, height / 2, sin * outerRadius);
            vertices[baseIndex + 5] = new Vector3(cos * innerRadius, height / 2, sin * innerRadius);
            vertices[baseIndex + 6] = new Vector3(cos * outerRadius, -height / 2, sin * outerRadius);
            vertices[baseIndex + 7] = new Vector3(cos * innerRadius, -height / 2, sin * innerRadius);
        }
        for (int i = 0; i < segments; i++){
            int current = i * 8;
            int next = ((i + 1) % segments) * 8;
            int triIndex = i * 24;

            // Outer wall
            triangles[triIndex + 0] = current + 0;
            triangles[triIndex + 1] = next + 0;
            triangles[triIndex + 2] = current + 1;
            triangles[triIndex + 3] = next + 0;
            triangles[triIndex + 4] = next + 1;
            triangles[triIndex + 5] = current + 1;

            // Inner wall
            triangles[triIndex + 6] = current + 2;
            triangles[triIndex + 7] = current + 3;
            triangles[triIndex + 8] = next + 2;
            triangles[triIndex + 9] = next + 2;
            triangles[triIndex + 10] = current + 3;
            triangles[triIndex + 11] = next + 3;

            // Top ring
            triangles[triIndex + 12] = current + 4;
            triangles[triIndex + 13] = current + 5;
            triangles[triIndex + 14] = next + 4;
            triangles[triIndex + 15] = next + 4;
            triangles[triIndex + 16] = current + 5;
            triangles[triIndex + 17] = next + 5;

            // Bottom ring
            triangles[triIndex + 18] = current + 6;
            triangles[triIndex + 19] = next + 6;
            triangles[triIndex + 20] = current + 7;
            triangles[triIndex + 21] = next + 6;
            triangles[triIndex + 22] = next + 7;
            triangles[triIndex + 23] = current + 7;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void CreateTriangularPrism(Vector3 position, bool tall, bool hollow, bool isWhite){
        GameObject prism = new GameObject((hollow ? "Hollow" : "Solid") + (isWhite ? "White" : "Black") + "Prism");
        prism.transform.position = position;
        prism.transform.rotation = Quaternion.Euler(-90, 0, 0);

        MeshFilter meshFilter = prism.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = prism.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = prism.AddComponent<MeshCollider>();

        Material material;
        if (hollow){
            material = isWhite ? whiteMaterialHollow : blackMaterialHollow;
            meshFilter.mesh = CreateTriangularPrismMesh();
        }
        else{
            material = isWhite ? whiteMaterial : blackMaterial;
            meshFilter.mesh = CreateTriangularPrismMesh();
        }

        meshCollider.sharedMesh = meshFilter.mesh;
        prism.transform.localScale = tall ? new Vector3(1f, 0.5f, 3f) : new Vector3(1f, 0.5f, 1f);
        meshRenderer.material = material;
    }

    Mesh CreateTriangularPrismMesh(){
        Mesh mesh = new Mesh();
        float h = 2f;
        float outerW = 0.5f;    
        float outerD = 0.4f;   
        float innerW = 0.25f;
        float innerD = 0.2f; 

        Vector3[] vertices = new Vector3[]{
            // Bottom outer triangle
            new Vector3(0, 0, outerD),
            new Vector3(-outerW, 0, -outerD),
            new Vector3(outerW, 0, -outerD),
            
            // Top outer triangle
            new Vector3(0, h, outerD),
            new Vector3(-outerW, h, -outerD),
            new Vector3(outerW, h, -outerD),
            
            // Bottom inner triangle
            new Vector3(0, 0, innerD),
            new Vector3(-innerW, 0, -innerD),
            new Vector3(innerW, 0, -innerD),
            
            // Top inner triangle
            new Vector3(0, h, innerD),
            new Vector3(-innerW, h, -innerD),
            new Vector3(innerW, h, -innerD),
            
            // Front-left outer wall
            new Vector3(0, 0, outerD),
            new Vector3(-outerW, 0, -outerD),
            new Vector3(0, h, outerD),
            new Vector3(-outerW, h, -outerD),
            
            // Front-right outer wall
            new Vector3(0, 0, outerD),
            new Vector3(outerW, 0, -outerD),
            new Vector3(0, h, outerD),
            new Vector3(outerW, h, -outerD),
            
            // Back outer wall
            new Vector3(-outerW, 0, -outerD),
            new Vector3(outerW, 0, -outerD),
            new Vector3(-outerW, h, -outerD),
            new Vector3(outerW, h, -outerD),
            
            // Front-left inner wall
            new Vector3(0, 0, innerD),
            new Vector3(-innerW, 0, -innerD),
            new Vector3(0, h, innerD),
            new Vector3(-innerW, h, -innerD),
            
            // Front-right inner wall
            new Vector3(0, 0, innerD),
            new Vector3(innerW, 0, -innerD),
            new Vector3(0, h, innerD),
            new Vector3(innerW, h, -innerD),
            
            // Back inner wall
            new Vector3(-innerW, 0, -innerD),
            new Vector3(innerW, 0, -innerD),
            new Vector3(-innerW, h, -innerD),
            new Vector3(innerW, h, -innerD),
            
            // Front edge
            new Vector3(0, h, outerD),
            new Vector3(0, h, innerD),
            
            // Back-left edge
            new Vector3(-outerW, h, -outerD),
            new Vector3(-innerW, h, -innerD),
            
            // Back-right edge
            new Vector3(outerW, h, -outerD),
            new Vector3(innerW, h, -innerD),
            
            // Front edge
            new Vector3(0, 0, outerD),
            new Vector3(0, 0, innerD),
            
            // Back-left edge
            new Vector3(-outerW, 0, -outerD),
            new Vector3(-innerW, 0, -innerD),
            
            // Back-right edge
            new Vector3(outerW, 0, -outerD),
            new Vector3(innerW, 0, -innerD),
        };
        int[] triangles = new int[]{
            // Outer front-left wall
            12, 14, 15,
            12, 15, 13,
        
            // Outer front-right wall
            16, 17, 19,
            16, 19, 18,
        
            // Outer back wall
            20, 22, 23,
            20, 23, 21,
        
            // Inner front-left wall
            24, 27, 26,
            24, 25, 27,
        
            // Inner front-right wall
            28, 30, 31,
            28, 31, 29,
        
            // Inner back wall
            32, 35, 34,
            32, 33, 35,
        
            // Top ring - front-left section
            36, 37, 39,
            36, 39, 38,
        
            // Top ring - front-right section
            36, 40, 41,
            36, 41, 37,
        
            // Top ring - back section
            38, 39, 41,
            38, 41, 40,
        
            // Bottom ring - front-left section
            42, 45, 43,
            42, 44, 45,
        
            // Bottom ring - front-right section
            42, 43, 47,
            42, 47, 46,
        
            // Bottom ring - back section
            44, 47, 45,
            44, 46, 47,
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}