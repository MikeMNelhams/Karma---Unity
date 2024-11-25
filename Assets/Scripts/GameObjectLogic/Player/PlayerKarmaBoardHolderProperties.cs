using UnityEngine;

public class PlayerKarmaBoardHolderProperties : MonoBehaviour
{
    [SerializeField] KarmaUpPilesHandler _upPilesHandler;
    [SerializeField] KarmaDownPilesHandler _downPilesHandler;
    [SerializeField] MeshRenderer _holderCuboidRenderer;

    public KarmaUpPilesHandler UpPilesHandler { get => _upPilesHandler; }
    public KarmaDownPilesHandler DownPilesHandler { get => _downPilesHandler; }
    public MeshRenderer HolderCuboidRenderer { get => _holderCuboidRenderer; }

    public void Destroy()
    {
        Destroy(UpPilesHandler.gameObject);
        Destroy(DownPilesHandler.gameObject);
        Destroy(gameObject);
    }
}
