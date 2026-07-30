using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _startGame;
    [SerializeField] private Button _tutorial;
    [SerializeField] private Button _credits;
    [SerializeField] private Button _backCredits;
    [SerializeField] private Button _backTutorial;

    [SerializeField] private CinemachineCamera _tutorialCamera;
    [SerializeField] private CinemachineCamera _creditsCamera;
    [SerializeField] private CinemachineCamera _mainCamera;

    private void Awake()
    {
        _startGame.onClick.AddListener(StartGame);
        _tutorial.onClick.AddListener(Tutorial);
        _credits.onClick.AddListener(Credits);

        _backTutorial.onClick.AddListener(BackTutorial);
        _backCredits.onClick.AddListener(BackCredits);
    }

    private void StartGame()
    {
        ChangeSceneController.Instance.LoadSceneByIndex(0);
        _mainCamera.Priority = 10;
    }

    private void Tutorial()
    {
        _tutorialCamera.Priority = 10;
    }

    private void BackTutorial()
    {
        _tutorialCamera.Priority = 0;
    }

    private void Credits()
    {
        _creditsCamera.Priority = 10;
    }

    private void BackCredits()
    {
        _creditsCamera.Priority = 0;
    }
}
