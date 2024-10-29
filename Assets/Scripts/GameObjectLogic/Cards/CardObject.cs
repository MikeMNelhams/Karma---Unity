using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using KarmaLogic.Cards;
using CardVisibility;
using System.Linq;

public class CardObject : SelectableCard, IEquatable<CardObject>, ICardVisibilityHandler
{
    [SerializeField] Material _frontMaterial; // This will get overwritten at runtime, for each individual card.
    [SerializeField] Material _selectedMaterial;
    [SerializeField] MeshRenderer _frontMeshRenderer;
    [SerializeField] MeshRenderer _cardMeshRenderer;

    Material _frontMaterialCopy;
    Material _selectedMaterialCopy;

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

    public void EnableSelectShader(Color color)
    {
        if (_frontMaterialCopy == null) { return; }
        _frontMaterialCopy.SetColor("_fresnelColor", color);
        _frontMaterialCopy.SetFloat("_isHighlighted", 1.0f); 
    }

    public override void ColorCardBorder(Color color)
    {
        _selectedMaterialCopy.SetColor("_color", color);
        _selectedMaterialCopy.SetFloat("_isEnabled", 1.0f);
    }

    public override void ResetCardBorder()
    {
        _selectedMaterialCopy.SetColor("_color", Color.black);
        _selectedMaterialCopy.SetFloat("_isEnabled", 0.0f);
    }

    public override void DisableSelectShader()
    {
        if (_frontMaterialCopy == null) { return; }
        _frontMaterialCopy.SetFloat("_isHighlighted", 0.0f);
    }

    public override void ToggleSelectShader()
    {
        if (_frontMaterialCopy == null) { return; }
        float fresnelIsEnabled = _frontMaterialCopy.GetFloat("_isHighlighted");
        _frontMaterialCopy.SetFloat("_isHighlighted", 1 - fresnelIsEnabled);
        //_selectedMaterialCopy.SetFloat("_isEnabled", 1 - fresnelIsEnabled); // I'm not sure if I prefer with or without this!
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
}
