using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;

    Player _player;

   public void Bind(Player player)
    {
        _player = player;
    }


    // todo not performant way to do this
    void Update()
    {
        if (_player)
            _scoreText.SetText(_player.Coins.ToString());
    }
}
