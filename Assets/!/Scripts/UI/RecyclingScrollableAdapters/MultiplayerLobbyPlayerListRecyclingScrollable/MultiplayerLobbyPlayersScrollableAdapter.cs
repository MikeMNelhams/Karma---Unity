using UnityEngine;

namespace CustomUI.RecyclingScrollable
{
    public class MultiplayerLobbyPlayersScrollableAdapter : RecyclingScrollableAdapter
    {
        int _playersInLobbyCount = 4;

        public override int ItemCount => _playersInLobbyCount;

        public override int SelectedItemIndex { get => 0; set { return; } }

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            holder.SetActive(true);
            holder.SetText("Waiting for player...");
        }

        public override ViewHolder OnCreateViewHolder(RectTransform parentRectTransform, GameObject viewHolderPrefab)
        {
            GameObject viewHolderGameObject = GameObject.Instantiate(viewHolderPrefab, parentRectTransform.gameObject.transform);

            if (!viewHolderGameObject.TryGetComponent<ScrollElement>(out var scrollElement)) { throw new MissingComponentException(); }
            viewHolderGameObject.SetActive(false);

            scrollElement.SetWidth(parentRectTransform.rect.width);
            scrollElement.SetLocalXPosition(0);

            return new ViewHolder(scrollElement);
        }
    }
}