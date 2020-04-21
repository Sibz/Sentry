using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Sibz.NetCode;
using Sibz.NetCode.WorldExtensions;
using Sibz.Sentry.Client;
using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby.Server;
using Unity.Entities;
using Unity.UIElements.Runtime;
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
        private Button Refresh => visualTree?.Q<Button>("Refresh");
        private VisualElement ConnectedArea => visualTree?.Q("ConnectedArea");
        private VisualElement NotConnectedArea => visualTree?.Q("NotConnectedArea");

        private VisualElement ListArea => visualTree?.Q("ListArea");

        private bool runRefresh;
        private bool refreshList;
        private Coroutine refreshConnectionState;

        private LobbyServer lobbyServer;
        private LobbyClient lobbyClient;

        // Start is called before the first frame update
        void Start()
        {
            lobbyServer = new LobbyServer(()=>
                NotConnectedArea.style.display = DisplayStyle.Flex);
            visualTree = GetComponent<PanelRenderer>().visualTree;
            CreateGame.RegisterCallback<ClickEvent>(OnCreateGameClick);
            //Refresh.RegisterCallback<ClickEvent>(OnRefreshClick);
            Connect.RegisterCallback<ClickEvent>(OnConnectClick);
            Disconnect.RegisterCallback<ClickEvent>(OnDisconnectClick);
            visualTree.RegisterCallback<ClickEvent>(OnClick);
            ConnectedArea.style.display = DisplayStyle.None;
            NotConnectedArea.style.display = DisplayStyle.None;
            runRefresh = true;

            //refreshConnectionState = StartCoroutine(CheckLobbyConnectionState());
        }

        private async void OnClick(ClickEvent evt)
        {
            if (evt.target is Button b && b.ClassListContains("destroy") &&
                b.parent.parent.parent is VisualElement parent)
            {
                Debug.Log($"Destroying game {(int) parent.userData}");
                /*client.DestroyGame((int)parent.userData);*/
                //await RefreshList(0.5f);
            }
        }

        private IEnumerator CheckLobbyConnectionState()
        {
            while (runRefresh)
            {
                yield return new WaitForSeconds(0.2f);
                /*if (client.IsConnected)
                {
                    ConnectedArea.style.display = DisplayStyle.Flex;
                    NotConnectedArea.style.display = DisplayStyle.None;
                }
                else
                {
                    ConnectedArea.style.display = DisplayStyle.None;
                    NotConnectedArea.style.display = DisplayStyle.Flex;
                    refreshList = false;
                }*/
            }
        }

        private async void OnCreateGameClick(ClickEvent evt)
        {
            lobbyClient.CreateNewGame(GameName.text);
            GameName.value = "";
            //await RefreshList(0.2f);
        }

        /*private async void OnRefreshClick(ClickEvent evt)
        {
            await RefreshList();
        }


        private async Task RefreshList(float delay = 0f)
        {
            if (delay > 0f)
            {
                var t = new Task(() => Thread.Sleep((int) (delay * 1000)));
                t.Start();
                await t;
            }

            if (!refreshList)
                return;
            /*IEnumerable<GameInfoComponent> items = client.GetItems();
            ListArea.Clear();
            foreach (GameInfoComponent gameInfoComponent in items)
            {
                VisualElement item = new VisualElement();
                NewGameItemTemplate.CloneTree(item);
                if (item.Q(null, "game-name") is Label l)
                    l.text = gameInfoComponent.Name.ToString();
                item.userData = gameInfoComponent.Id;
                ListArea.Add(item);
            }#1#
        }*/

        private void RefreshList()
        {
            foreach (GameInfoComponent gameInfoComponent in lobbyClient.Games)
            {
                VisualElement item = new VisualElement();
                NewGameItemTemplate.CloneTree(item);
                if (item.Q(null, "game-name") is Label l)
                    l.text = gameInfoComponent.Name.ToString();
                item.userData = gameInfoComponent.Id;
                ListArea.Add(item);
            }
        }

        private LobbyClient CreateClientWorld()
        {
            LobbyClient world = new LobbyClient();
            world.Connected += i =>
            {
                Debug.Log("Connected");
                ConnectedArea.style.display = DisplayStyle.Flex;
                NotConnectedArea.style.display = DisplayStyle.None;
            };
            world.Disconnected += () =>
            {
                Debug.Log("Disconnected");
                ConnectedArea.style.display = DisplayStyle.None;
                NotConnectedArea.style.display = DisplayStyle.Flex;
                ListArea.Clear();
                lobbyClient.DestroyWorld();
            };
            world.WorldDestroyed += () => lobbyClient = null;
            world.Connecting += () => Debug.Log("Connecting...");
            world.ConnectionFailed += s => Debug.Log($"Connect Failed: {s}");
            world.GameListUpdated += RefreshList;
            return world;
        }

        private async void OnConnectClick(ClickEvent evt)
        {
            ListArea.Clear();

            lobbyClient = lobbyClient ?? CreateClientWorld();

            if (!lobbyClient.WorldIsCreated)
            {
                lobbyClient.CreateWorld();
            }

            lobbyClient.Connect();

            /*client.CreateLobbyWorld();
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
            }).Start();*/
        }

        private async void OnDisconnectClick(ClickEvent evt)
        {
            lobbyClient.Disconnect();
        }

        private void OnDisable()
        {
            CreateGame.UnregisterCallback<ClickEvent>(OnCreateGameClick);
            //Refresh.UnregisterCallback<ClickEvent>(OnRefreshClick);
            Connect.UnregisterCallback<ClickEvent>(OnConnectClick);
            runRefresh = false;
            refreshList = false;
            if (refreshConnectionState != null)
            {
                StopCoroutine(refreshConnectionState);
            }
        }

        private void OnDestroy()
        {
            if (lobbyServer != null && lobbyServer.WorldIsCreated)
            {
                lobbyServer.Dispose();
            }

            if (lobbyClient != null && lobbyClient.WorldIsCreated)
            {
                lobbyClient.Dispose();
            }
        }
    }
}