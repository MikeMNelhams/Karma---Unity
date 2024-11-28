using UnityEngine;


public class CardLegalityHinter
{
    bool _areLegalHintsEnabled = false;
    PlayerHandler _playerHandler;

    public CardLegalityHinter(PlayerHandler playerHandler, bool areLegalHintsEnabled = false)
    {
        _areLegalHintsEnabled = areLegalHintsEnabled;
        _playerHandler = playerHandler;
    }
}
