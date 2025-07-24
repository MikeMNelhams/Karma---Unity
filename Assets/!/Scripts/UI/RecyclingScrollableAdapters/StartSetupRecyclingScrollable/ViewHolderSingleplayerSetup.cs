namespace CustomUI.RecyclingScrollable.SingleplayerSetup
{
    [System.Serializable]
    public class ViewHolderSingleplayerSetup : ViewHolder
    {
        readonly SingleplayerSetupScrollElement _scrollElement;

        public ViewHolderSingleplayerSetup(SingleplayerSetupScrollElement element) : base(element)
        {
            _scrollElement = element;
        }

        public void SetText(string text)
        {
            _scrollElement.SetText(text);
        }

        public void RegisterOnClickListener(ScrollElement.OnClickListener listener)
        {
            _scrollElement.RegisterOnClickListener(listener);
        }
    }
}