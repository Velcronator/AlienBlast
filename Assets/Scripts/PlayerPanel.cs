using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Image [] _hearts;
    [SerializeField] Image _flashImage; 
    [SerializeField] float _flashTime = 0.5f;

    private WaitForSeconds _waitForFlashTime;

    Player _player;
    private void Awake()
    {   // Cache the WaitForSeconds object to avoid creating it every time we need it
        _waitForFlashTime = new WaitForSeconds(_flashTime);
    }

    public void Bind(Player player)
    {
        _player = player;
        _player.CoinsChanged += UndateCoins;
        _player.HealthChanged += UpdateHealth;
        UndateCoins();
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            Image heart = _hearts[i];
            heart.enabled = i < _player.Health;
        }
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        _flashImage.enabled = true;
        yield return _waitForFlashTime;
        _flashImage.enabled = false;
    }

    private void UndateCoins()
    {
        _scoreText.SetText(_player.Coins.ToString());
    }
}
