using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby.Client;
using Unity.UIElements.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.Sentry
{

    public class LobbyManager : MonoBehaviour
    {
        public VisualTreeAsset NewGameItemTemplate;

        private VisualElement visualTree;

        private Button Connect => visualTree?.Q<Button>("Connect");
        private Button Disconnect => visualTree?.Q<Button>("Disconnect");
        private Button CreateGame => visualTree?.Q<Button>("CreateGame");
        private TextField GameName => visualTree?.Q<TextField>("GameName");
        private  Button Refresh => visualTree?.Q<Button>("Refresh");
        private VisualElement ConnectedArea => visualTree?.Q("ConnectedArea");
        private VisualElement NotConnectedArea => visualTree?.Q("NotConnectedArea");

        private VisualElement ListArea => visualTree?.Q("ListArea");

        private readonly LobbyClient client = new LobbyClient();

        private bool runRefresh;
        private bool refreshList;
        private Coroutine refreshConnectionState;

        // Start is called before the first frame update
        void Start()
        {
            visualTree = GetComponent<PanelRenderer>().visualTree;
            CreateGame.RegisterCallback<ClickEvent>(OnCreateGameClick);
            Refresh.RegisterCallback<ClickEvent>(OnRefreshClick);
            Connect.RegisterCallback<ClickEvent>(OnConnectClick);
            Disconnect.RegisterCallback<ClickEvent>(OnDisconnectClick);
            visualTree.RegisterCallback<ClickEvent>(OnClick);
            ConnectedArea.style.display = DisplayStyle.None;
            NotConnectedArea.style.display = DisplayStyle.None;
            runRefresh = true;
            refreshConnectionState = StartCoroutine(CheckLobbyConnectionState());
        }

        private async void OnClick(ClickEvent evt)
        {
            if (evt.target is Button b && b.ClassListContains("destroy") && b.parent.Q<IntegerField>() is IntegerField integerField)
            {
                Debug.Log($"Destroying game {integerField.value}");
                client.DestroyGame(integerField.value);
                await RefreshList(0.5f);
            }
        }

        private IEnumerator CheckLobbyConnectionState()
        {
            while (runRefresh)
            {
                yield return new WaitForSeconds(0.2f);
                if (client.IsConnected)
                {
                    ConnectedArea.style.display = DisplayStyle.Flex;
                    NotConnectedArea.style.display = DisplayStyle.None;
                }
                else
                {
                    ConnectedArea.style.display = DisplayStyle.None;
                    NotConnectedArea.style.display = DisplayStyle.Flex;
                    refreshList = false;
                }
            }
        }

        private async void OnCreateGameClick(ClickEvent evt)
        {
            client.CreateNewGame(GameName.text);
            GameName.value = "";
            await RefreshList(0.2f);
        }

        private async void OnRefreshClick(ClickEvent evt)
        {
            await RefreshList();
        }


        private async Task RefreshList(float delay = 0f)
        {
            if (delay > 0f)
            {
                var t  =new Task(() => Thread.Sleep((int)(delay*1000)));
                t.Start();
                await t;
            }
            if (!refreshList)
                return;
            IEnumerable<GameInfoComponent> items = client.GetItems();
            ListArea.Clear();
            foreach (GameInfoComponent gameInfoComponent in items)
            {
                VisualElement item = new VisualElement();
                NewGameItemTemplate.CloneTree(item);
                if (item.Q(null, "game-name") is Label l)
                    l.text = gameInfoComponent.Name.ToString();
                if (item.Q<IntegerField>() is IntegerField i)
                    i.value = gameInfoComponent.Id;
                ListArea.Add(item);
            }
        }

        private async void OnConnectClick(ClickEvent evt)
        {
            ListArea.Clear();
            client.CreateLobbyWorld();
            client.ConnectToServerLobby();
            await RefreshList(0.1f);
            await RefreshList(0.5f);
            refreshList = true;
            new Task(async () =>
            {
                while (refreshList)
                {
                    await RefreshList(1f);
                }
            }).Start();
        }
        private async void OnDisconnectClick(ClickEvent evt)
        {
            ListArea.Clear();
            refreshList = false;
            await client.DisconnectAsync(true);
        }


        private void OnDisable()
        {
            CreateGame.UnregisterCallback<ClickEvent>(OnCreateGameClick);
            Refresh.UnregisterCallback<ClickEvent>(OnRefreshClick);
            Connect.UnregisterCallback<ClickEvent>(OnConnectClick);
            runRefresh = false;
            refreshList = false;
            StopCoroutine(refreshConnectionState);
        }
    }
}