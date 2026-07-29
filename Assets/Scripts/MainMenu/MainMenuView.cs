using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _startGame;
    //[SerializeField] private Button _options;
    [SerializeField] private Button _credits;

    private void Awake()
    {
        _startGame.onClick.AddListener(StartGame);
        //_options.onClick.AddListener(Options);
        _credits.onClick.AddListener(Credits);
    }

    private void StartGame()
    {
        ChangeSceneController.Instance.LoadSceneByIndex(0);
    }

    //private void Options()
    //{

    //}

    private void Credits()
    {

    }
}
