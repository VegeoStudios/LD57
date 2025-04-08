using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private GameObject[] _screens;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("Canvas component not found on PauseMenu.");
            return;
        }
    }

    private void Update()
    {
        if (VictoryScreen.Activated) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (_screens.Length > 0) _screens[0].SetActive(true);
        for (int i = 1; i < _screens.Length; i++)
        {
            _screens[i].SetActive(false);
        }

        if (_canvas != null)
        {
            _canvas.enabled = !_canvas.enabled;
            Time.timeScale = _canvas.enabled ? 0 : 1;
            // Pause audio
            AudioListener.pause = _canvas.enabled;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1; // Resume the game before restarting
    }
}
