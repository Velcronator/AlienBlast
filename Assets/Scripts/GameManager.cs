using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GetComponent<PlayerInputManager>().onPlayerJoined += HandlePlayerJoined;
    }

    void HandlePlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("HandlePlayerJoined " + playerInput);
        PlayerData playerData = GetPlayerData(playerInput.playerIndex);

        Player player = playerInput.GetComponent<Player>();
        player.Bind(playerData);
    }

    private PlayerData GetPlayerData(int playerIndex)
    {
        if (_playerDatas.Count <= playerIndex)
        {
            var playerData = new PlayerData();
            _playerDatas.Add(playerData);
        }
        return _playerDatas[playerIndex];
    }

    [SerializeField] List<PlayerData> _playerDatas = new List<PlayerData>();
}