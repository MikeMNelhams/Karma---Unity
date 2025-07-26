using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUI
{
    [AddComponentMenu("UI/ButtonAsync", 30)]
    public class ButtonAwaitable : Button
    {
#pragma warning disable IDE1006 // Naming Styles
        public new ButtonClickedEventAsync onClick { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

        public ButtonAwaitable() : base()
        {
            onClick = new ButtonClickedEventAsync();
        }

        public override async void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            await onClick.Invoke();
        }

        public override async void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            await onClick.Invoke();
        }
    }

    public class ButtonClickedEventAsync
    {
        List<UnityAction> _callbacks;
        List<Func<Task>> _awaitableCallbacks;

        public ButtonClickedEventAsync()
        {
            _callbacks = new List<UnityAction>();
            _awaitableCallbacks = new List<Func<Task>>();
        }

        public void AddListener(UnityAction callback)
        {
            _callbacks.Add(callback);
        }

        public void RemoveListener(UnityAction callback)
        {
            _callbacks.Remove(callback);
        }

        public void AddListener(Func<Task> callback)
        {
            _awaitableCallbacks.Add(callback);
        }

        public void RemoveListener(Func<Task> callback)
        {
            _awaitableCallbacks.Remove(callback);
        }

        public async Task Invoke()
        {
            UnityEngine.Debug.Log("Button Invoke called for: " + ToString());
            _callbacks ??= new List<UnityAction>();
            foreach (UnityAction callback in _callbacks)
            {
                callback?.Invoke();
            }

            _awaitableCallbacks ??= new List<Func<Task>>();
            foreach (Func<Task> callback in _awaitableCallbacks)
            {
                await callback?.Invoke();
            }
        }
    }
}



