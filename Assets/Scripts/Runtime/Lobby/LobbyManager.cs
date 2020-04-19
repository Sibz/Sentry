using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Sibz.NetCode;
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
        private  Button Refresh => visualTree?.Q<Button>("Refresh");
        private VisualElement ConnectedArea => visualTree?.Q("ConnectedArea");
        private VisualElement NotConnectedArea => visualTree?.Q("NotConnectedArea");

        private VisualElement ListArea => visualTree?.Q("ListArea");

        private bool runRefresh;
        private bool refreshList;
        private Coroutine refreshConnectionState;

        private ServerWorld serverWorld;
        private ClientWorld clientWorld;

        // Start is called before the first frame update
        void Start()
        {
            serverWorld = new ServerWorld(new ServerOptions()
            {
                WorldName = "Lobby Server",
                CreateWorldOnInstantiate = true,
                Address = "0.0.0.0",
                Port = 2165,
                GhostCollectionPrefab = Resources.Load<GameObject>("Collection")
            });
            serverWorld.ListenSuccess += () => NotConnectedArea.style.display = DisplayStyle.Flex;
            serverWorld.Listen();
            visualTree = GetComponent<PanelRenderer>().visualTree;
            CreateGame.RegisterCallback<ClickEvent>(OnCreateGameClick);
            Refresh.RegisterCallback<ClickEvent>(OnRefreshClick);
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
            if (evt.target is Button b && b.ClassListContains("destroy") && b.parent.parent.parent is VisualElement parent)
            {
                Debug.Log($"Destroying game {(int)parent.userData}");
                /*client.DestroyGame((int)parent.userData);*/
                await RefreshList(0.5f);
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
            /*client.CreateNewGame(GameName.text);*/
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
            }*/
        }

        private ClientWorld CreateClientWorld()
        {
            ClientWorld world = new ClientWorld(new ClientOptions()
            {
                WorldName = "Lobby Client",
                Port = 2165,
                Address = "127.0.0.1",
                GhostCollectionPrefab = Resources.Load<GameObject>("Collection")
            });
            world.Connected += i =>
            {
                ConnectedArea.style.display = DisplayStyle.Flex;
                NotConnectedArea.style.display = DisplayStyle.None;
            };
            world.Disconnected += () =>
            {
                ConnectedArea.style.display = DisplayStyle.None;
                NotConnectedArea.style.display = DisplayStyle.Flex;
                ListArea.Clear();
                clientWorld.Dispose();
                clientWorld = null;
            };
            world.Connecting += () => Debug.Log("Connecting...");
            world.ConnectionFailed += s => Debug.Log($"Connect Failed: {s}");
            return world;
        }

        private async void OnConnectClick(ClickEvent evt)
        {
            ListArea.Clear();

            clientWorld = clientWorld ?? CreateClientWorld();

            if (!clientWorld.World.IsCreated)
            {
                clientWorld.CreateWorld();
            }

            clientWorld.Connect();
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
            clientWorld.Disconnect();
        }


        private void OnDisable()
        {
            CreateGame.UnregisterCallback<ClickEvent>(OnCreateGameClick);
            Refresh.UnregisterCallback<ClickEvent>(OnRefreshClick);
            Connect.UnregisterCallback<ClickEvent>(OnConnectClick);
            runRefresh = false;
            refreshList = false;
            if (refreshConnectionState!=null)
            {
                StopCoroutine(refreshConnectionState);
            }
        }
    }
}