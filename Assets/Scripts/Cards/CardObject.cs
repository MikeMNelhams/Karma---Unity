using UnityEngine;
using System.IO;
using System;
using Karma.Cards;


public class CardObject : MonoBehaviour
{
    public Material planeMaterial;
    public GameObject quad;
    public Card CurrentCard { get; protected set; }
    public event Action<Card> OnCardClick;

    public void UpdateImage(Card card)
    {
        CurrentCard = card;
        // "/Assets/Resources/Cards/Clubs/Jack.png"
        // Only on Windows: https://docs.unity3d.com/ScriptReference/Application-dataPath.html
        string assetsPath = Application.dataPath;
        string cardName = Enum.GetName(typeof(CardValue), card.value);
        cardName = cardName[0].ToString().ToUpper() + cardName.Substring(1).ToLower();
        string resourcePath = assetsPath + "/Resources/Cards/" + card.suit.name + "/" + cardName + ".png";
        if (File.Exists(resourcePath))
        {
            // Image file exists - load bytes into texture
            var bytes = File.ReadAllBytes(resourcePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            Material materialCopy = new Material(planeMaterial);
            materialCopy.mainTexture = tex;
            // Apply to Plane
            MeshRenderer mr = quad.GetComponent<MeshRenderer>();
            mr.material = materialCopy;
        } 
        else
        {
            print("File path: " + resourcePath + " does not exist!");
        }
    }

    void OnMouseDown()
    {
        OnCardClick.Invoke(CurrentCard);
    }
}
