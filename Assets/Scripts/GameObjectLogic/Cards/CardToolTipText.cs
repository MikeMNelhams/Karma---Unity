using KarmaLogic.Cards;

namespace CardToolTips
{
    public class CardToolTipText
    {
        public int ID { get; private set; }
        public CardValue CardValue { get; private set; }
        public string CardEffectText { get; private set; }
        public string CardPlayRequirements { get; private set; }

        public CardToolTipText(string ID, string cardValue, string cardEffectText, string cardPlayRequirements)
        {
            this.ID = int.Parse(ID);
            this.CardValue = (CardValue) int.Parse(cardValue);
            this.CardEffectText = cardEffectText;
            this.CardPlayRequirements = cardPlayRequirements;
        }
    }
}