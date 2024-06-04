using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFrontBackRenderer : MonoBehaviour
{
    public Material planeMaterial;
    public GameObject quad;

    // Start is called before the first frame update
    void Start()
    {
        var filePath = "E:/MichaelsStuff/Coding/UnityProjects/Karma/Karma/Assets/Resources/Cards/Clubs/Jack.png";
        if (System.IO.File.Exists(filePath))
        {
            // Image file exists - load bytes into texture
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            planeMaterial.mainTexture = tex;
            // Apply to Plane
            MeshRenderer mr = quad.GetComponent<MeshRenderer>();
            mr.material = planeMaterial;
        }
    }
}
