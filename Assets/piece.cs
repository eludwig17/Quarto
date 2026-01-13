
using UnityEngine;

public class QuartoGenerator : MonoBehaviour
{
    public Material whiteMaterial;
    public Material blackMaterial;

    void Start()
    {
        CreateCylinder(new Vector3(0, 0, 0), true);  
        CreateCylinder(new Vector3(2, 0, 0), true);  
        CreateCylinder(new Vector3(4, 0, 0), true); 
        CreateCylinder(new Vector3(6, 0, 0), true);  

        CreateCylinder(new Vector3(0, 0, 2), false); 
        CreateCylinder(new Vector3(2, 0, 2), false);
        CreateCylinder(new Vector3(4, 0, 2), false); 
        CreateCylinder(new Vector3(6, 0, 2), false); 


        CreatePyramid(new Vector3(0, 0, 4), true, true);   
        CreatePyramid(new Vector3(2, 0, 4), true, false);  
        CreatePyramid(new Vector3(4, 0, 4), true, true);   
        CreatePyramid(new Vector3(6, 0, 4), true, false); 

        CreatePyramid(new Vector3(0, 0, 6), false, true); 
        CreatePyramid(new Vector3(2, 0, 6), false, false); 
        CreatePyramid(new Vector3(4, 0, 6), false, true);  
        CreatePyramid(new Vector3(6, 0, 6), false, false); 
    }

    void CreateCylinder(Vector3 position, bool tall)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;

        cylinder.transform.localScale = tall ? new Vector3(1f, 1.2f, 1f) : new Vector3(1f, 0.6f, 1f);

        MakeHollow(cylinder);

        cylinder.GetComponent<Renderer>().material = whiteMaterial;
    }


    void CreatePyramid(Vector3 position, bool tall, bool hollow)
    {
        GameObject pyramid = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pyramid.transform.position = position;

        pyramid.transform.localScale = tall ? new Vector3(1f, 1.2f, 1f) : new Vector3(1f, 0.6f, 1f);

        if (hollow)
            MakeHollow(pyramid);

        pyramid.GetComponent<Renderer>().material = blackMaterial;
    }

    void MakeHollow(GameObject outer)
    {
        GameObject inner = GameObject.CreatePrimitive(PrimitiveType.Cylinder); 
        inner.transform.parent = outer.transform;
        inner.transform.localPosition = Vector3.zero;
        inner.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);

        inner.GetComponent<Renderer>().material.color = Color.black;
    }
}
