using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool CinematicPlaying { get; private set; }
    public static bool IsLoading { get; private set; }

    public List<string> AllGameNames = new List<string>();

    [SerializeField] GameData _gameData;

    PlayerInputManager _playerInputManager;
    public void ToggleCinematic(bool cinematicPlaying) => CinematicPlaying = cinematicPlaying;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += HandlePlayerJoined;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        string commaSeparatedList = PlayerPrefs.GetString("AllGameNames");
        Debug.Log(commaSeparatedList);
        AllGameNames = commaSeparatedList.Split(",").ToList();
        AllGameNames.Remove("");
    }

    void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        else
        {
            _gameData.CurrentLevelName = arg0.name;
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;

            var levelData = _gameData.LevelDatas.FirstOrDefault(t => t.LevelName == arg0.name);
            if (levelData == null)
            {
                levelData = new LevelData() { LevelName = arg0.name };
                _gameData.LevelDatas.Add(levelData);
            }

            Bind<Coin, CoinData>(levelData.CoinDatas);
            Bind<LaserSwitch, LaserSwitchData>(levelData.LaserSwitchDatas);
            Bind<PlayerInventory, PlayerData>(_gameData.PlayerDatas);

            var allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (var player in allPlayers)
            {
                var playerInput = player.GetComponent<PlayerInput>();
                var data = GetPlayerData(playerInput.playerIndex);
                player.Bind(data);
                if (GameManager.IsLoading)
                {
                    player.RestorePositionAndVelocity();
                    IsLoading = false;
                }
            }
            //SaveGame();
        }

    }

    void Bind<T, D>(List<D> datas) where T : MonoBehaviour, IBind<D> where D : INamed, new()
    {
        var instances = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (var instance in instances)
        {
            var data = datas.FirstOrDefault(t => t.Name == instance.name);
            if (data == null)
            {
                data = new D() { Name = instance.name };
                datas.Add(data);
            }
            instance.Bind(data);
        }
    }

    public void SaveGame()
    {
        if (string.IsNullOrWhiteSpace(_gameData.GameName))
            _gameData.GameName = "Game " + AllGameNames.Count;

        string text = JsonUtility.ToJson(_gameData);
        Debug.Log(text);

        PlayerPrefs.SetString(_gameData.GameName, text);

        if (AllGameNames.Contains(_gameData.GameName) == false)
            AllGameNames.Add(_gameData.GameName);

        string commaSeparatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeparatedGameNames);
        PlayerPrefs.Save();
    }

    public void ReloadGame() => LoadGame(_gameData.GameName);

    public void LoadGame(string gameName)
    {
        IsLoading = true;
        string text = PlayerPrefs.GetString(gameName);
        _gameData = JsonUtility.FromJson<GameData>(text);
        if (String.IsNullOrWhiteSpace(_gameData.CurrentLevelName))
            _gameData.CurrentLevelName = "Level01";
        SceneManager.LoadScene(_gameData.CurrentLevelName);
    }

    void HandlePlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("HandlePlayerJoined " + playerInput);
        PlayerData playerData = GetPlayerData(playerInput.playerIndex);

        Player player = playerInput.GetComponent<Player>();
        player.Bind(playerData);
    }

    PlayerData GetPlayerData(int playerIndex)
    {
        if (_gameData.PlayerDatas.Count <= playerIndex)
        {
            var playerData = new PlayerData();
            _gameData.PlayerDatas.Add(playerData);
        }
        return _gameData.PlayerDatas[playerIndex];
    }

    public void NewGame()
    {
        Debug.Log("NewGame Called");
        _gameData = new GameData();
        _gameData.GameName = DateTime.Now.ToString("G");
        SceneManager.LoadScene("Level01");
    }

    public void DeleteGame(string gameName)
    {
        PlayerPrefs.DeleteKey(gameName);
        AllGameNames.Remove(gameName);
        string commaSeparatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeparatedGameNames);
        PlayerPrefs.Save();
    }

    internal Item GetItem(string itemName)
    {
        string prefabName = itemName.Substring(0, itemName.IndexOf("_"));
        var prefab = _allItems.FirstOrDefault(t => t.name == prefabName);
        if (prefab == null)
        {
            Debug.LogError($"Unable to find item {itemName}");
            return null;
        }
        var newInstance = Instantiate(prefab);
        newInstance.name = prefabName;
        return newInstance;
    }

    public List<Item> _allItems;
}