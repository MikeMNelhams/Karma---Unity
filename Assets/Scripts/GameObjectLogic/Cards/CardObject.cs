using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using KarmaLogic.Cards;
using CardVisibility;
using System.Linq;

public class CardObject : MonoBehaviour, IEquatable<CardObject>, ICardVisibilityHandler
{
    [SerializeField] Material _frontMaterial; // This will get overwritten at runtime, for each individual card.
    [SerializeField] Material _selectedMaterial;
    [SerializeField] MeshRenderer _frontMeshRenderer;
    [SerializeField] MeshRenderer _cardMeshRenderer;

    public Card CurrentCard { get; set; }

    public event Action<CardObject> OnCardClick;
    Material _frontMaterialCopy;
    Material _selectedMaterialCopy;

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

            _selectedMaterialCopy = new(_selectedMaterial);

            Material[] cardMaterials = _cardMeshRenderer.materials;
            Material[] cardMaterialsNew = new Material[cardMaterials.Length];
            cardMaterialsNew[0] = cardMaterials[0];
            cardMaterialsNew[1] = _selectedMaterialCopy;
            _cardMeshRenderer.materials = cardMaterialsNew;

            Material frontMaterialCopy = new(_frontMaterial) {  mainTexture = tex };
            _frontMeshRenderer.material = frontMaterialCopy;
            _frontMaterialCopy = frontMaterialCopy;
            SetCardName(card.ToString());
        } 
        else
        {
            throw new Exception("File path: " + resourcePath + " does not exist!");
        }
    }

    public void ToggleSelectShader()
    {
        if (_frontMaterialCopy == null) { return; }
        float fresnelIsEnabled = _frontMaterialCopy.GetFloat("_isHighlighted");
        _frontMaterialCopy.SetFloat("_isHighlighted", 1 - fresnelIsEnabled);
        //_selectedMaterialCopy.SetFloat("_isEnabled", 1 - fresnelIsEnabled); // I'm not sure if I prefer with or without this!
    }

    public void DisableSelectShader()
    {
        if (_frontMaterialCopy == null) { return; }
        _frontMaterialCopy.SetFloat("_isHighlighted", 0.0f);
    }

    void OnMouseDown()
    {
        if (OnCardClick == null) { return; }
        if (!EventSystem.current.IsPointerOverGameObject()) { return; }
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
