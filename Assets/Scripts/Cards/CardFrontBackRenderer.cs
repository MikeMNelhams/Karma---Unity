using UnityEngine;
using UnityEngine.Windows;
using Karma.Cards;


public class CardFrontBackRenderer : MonoBehaviour
{
    public Material planeMaterial;
    public GameObject quad;

    void UpdateImage(Card card)
    {
        // "/Assets/Resources/Cards/Clubs/Jack.png"
        string resourcePath = "Cards/" + card.suit.name + "/" + card.value + ".png";
        if (File.Exists(resourcePath))
        {
            // Image file exists - load bytes into texture
            var bytes = File.ReadAllBytes(resourcePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            planeMaterial.mainTexture = tex;
            // Apply to Plane
            MeshRenderer mr = quad.GetComponent<MeshRenderer>();
            mr.material = planeMaterial;
        } 
        else
        {
            print("File path: " + resourcePath + " does not exist!");
        }
        
    }
}
