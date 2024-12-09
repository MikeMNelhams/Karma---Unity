namespace CardVisibility
{
    public interface ICardVisibilityHandler
    {
        public bool IsVisible(int observerPlayerIndex);

        public bool IsOwnedBy(int observerPlayerIndex);
    }
}