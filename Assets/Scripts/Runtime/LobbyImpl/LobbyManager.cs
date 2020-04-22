using System.Collections;
using Sibz.Lobby.Client;
using Sibz.Lobby.Server;
using Sibz.Sentry.Components;
using Unity.Mathematics;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.Sentry.Lobby
{
    public class MyLobbyClient : LobbyClient<GameInfoComponent, CreateGameRequest>
    {
        public void CreateNewGame(string name)
        {
            base.CreateNewGame(new CreateGameRequest { Name = name, Size = new int2(3, 3) });
        }
    }
    internal class LobbyManager : MonoBehaviour
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

        private LobbyServer lobbyServer;
        private MyLobbyClient lobbyClient;

        void Start()
        {
            lobbyServer = new LobbyServer(()=>
                NotConnectedArea.style.display = DisplayStyle.Flex);
            visualTree = GetComponent<PanelRenderer>().visualTree;
            CreateGame.RegisterCallback<ClickEvent>(OnCreateGameClick);
            //Refresh.RegisterCallback<ClickEvent>(OnRefreshClick);
            Connect.RegisterCallback<ClickEvent>(OnConnectClick);
            Disconnect.RegisterCallback<ClickEvent>(OnDisconnectClick);
            ConnectedArea.style.display = DisplayStyle.None;
            NotConnectedArea.style.display = DisplayStyle.None;

        }

        private void OnCreateGameClick(ClickEvent evt)
        {
            lobbyClient.CreateNewGame(GameName.text);
            GameName.value = "";
        }

        private void RefreshList()
        {
            ListArea.Clear();
            foreach (GameInfoComponent gameInfoComponent in lobbyClient.Games)
            {
                VisualElement item = new VisualElement();
                NewGameItemTemplate.CloneTree(item);
                if (item.Q(null, "game-name") is Label l)
                    l.text = gameInfoComponent.Name.ToString();

                if (item.Q<Button>(null, "destroy") is Button b)
                {
                    b.RegisterCallback<ClickEvent>((e)=> lobbyClient.DestroyGame(gameInfoComponent.Id));
                }
                ListArea.Add(item);
            }
        }

        private MyLobbyClient CreateClientWorld()
        {
            MyLobbyClient world = new MyLobbyClient();
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

        private void OnConnectClick(ClickEvent evt)
        {
            ListArea.Clear();

            lobbyClient = lobbyClient ?? CreateClientWorld();

            if (!lobbyClient.WorldIsCreated)
            {
                lobbyClient.CreateWorld();
            }

            lobbyClient.Connect();
        }

        private void OnDisconnectClick(ClickEvent evt)
        {
            lobbyClient.Disconnect();
        }

        private void OnDisable()
        {
            CreateGame.UnregisterCallback<ClickEvent>(OnCreateGameClick);
            Connect.UnregisterCallback<ClickEvent>(OnConnectClick);
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