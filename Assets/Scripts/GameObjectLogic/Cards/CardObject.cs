using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using KarmaLogic.Cards;
using CardVisibility;

public class CardObject : MonoBehaviour, IEquatable<CardObject>, ICardVisibilityHandler
{
    public Material planeMaterial;
    public GameObject frontQuad;

    public Card CurrentCard { get; set; }

    public event Action<CardObject> OnCardClick;
    Material _frontMaterial;

    ICardVisibilityHandler _cardVisibilityHandler;

    public void SetCard(Card card)
    {
        CurrentCard = card;
        // "/Assets/Resources/Cards/Clubs/Jack.png"
        // Only on Windows: https://docs.unity3d.com/ScriptReference/Application-dataPath.html
        string assetsPath = Application.dataPath;
        string cardName = Enum.GetName(typeof(CardValue), card.Value);
        cardName = cardName[0].ToString().ToUpper() + cardName[1..].ToLower();
        string resourcePath = assetsPath + "/Resources/Cards/" + card.Suit._name + "/" + cardName + ".png";
        if (File.Exists(resourcePath))
        {
            // Image file exists - load bytes into texture
            var bytes = File.ReadAllBytes(resourcePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            Material materialCopy = new(planeMaterial) {  mainTexture = tex };
            MeshRenderer mr = frontQuad.GetComponent<MeshRenderer>();
            mr.material = materialCopy;
            _frontMaterial = materialCopy;
            SetCardName(card.ToString());
        } 
        else
        {
            print("File path: " + resourcePath + " does not exist!");
        }
    }

    public void ToggleSelectShader()
    {
        if (_frontMaterial == null) { return; }
        float fresnelIsEnabled = _frontMaterial.GetFloat("_isHighlighted");
        _frontMaterial.SetFloat("_isHighlighted", 1 - fresnelIsEnabled);
    }

    public void DisableSelectShader()
    {
        if (_frontMaterial == null) { return; }
        _frontMaterial.SetFloat("_isHighlighted", 0.0f);
    }

    void OnMouseDown()
    {
        if (OnCardClick == null) { return; }
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        ToggleSelectShader();
        OnCardClick.Invoke(this);
    }

    public void SetCardName(string name)
    {
        gameObject.name = name;
    }

    public bool Equals(CardObject other)
    {
        if (ReferenceEquals(this, other)) return true;
        return false;
    }

    public void SetParent(ICardVisibilityHandler cardVisibilityHandler, Transform parentTransform)
    {
        transform.parent = parentTransform;
        SetParent(cardVisibilityHandler);
    }

    public void SetParent(ICardVisibilityHandler cardVisibilityHandler)
    {
        _cardVisibilityHandler = cardVisibilityHandler;
    }

    public bool IsVisible(int observerPlayerIndex)
    {
        if (_cardVisibilityHandler == null)
        {
            throw new SystemException("Card has no parent set!");
        }

        return _cardVisibilityHandler.IsVisible(observerPlayerIndex);
    }
}
