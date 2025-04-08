using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    private static VictoryScreen _instance;

    public static bool Activated = false;

    private Canvas _canvas;
    [SerializeField] private Image _cover;
    [SerializeField] private TextMeshProUGUI[] _victoryTexts;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        _canvas = GetComponent<Canvas>();
    }

    public static void StartVictoryScreen()
    {
        if (Activated)
        {
            return;
        }
        Activated = true;
        _instance.StartCoroutine(_instance.VictoryRoutine());
    }

    private IEnumerator VictoryRoutine()
    {
        _cover.color = new Color(_cover.color.r, _cover.color.g, _cover.color.b, 0f);
        for (int i = 0; i < _victoryTexts.Length; i++)
        {
            _victoryTexts[i].color = new Color(_victoryTexts[i].color.r, _victoryTexts[i].color.g, _victoryTexts[i].color.b, 0f);
        }

        _canvas.enabled = true;

        ShipSystemsManager.Instance.ReactorModule.IsActive = false;

        string depthName = ShipSystemsManager.Instance.CurrentTierName;
        _victoryTexts[2].text = depthName;

        float depth = ShipSystemsManager.Instance.Depth;
        _victoryTexts[3].text = depth.ToString("F0") + "m";

        while (_cover.color.a < 1f)
        {
            _cover.color = new Color(_cover.color.r, _cover.color.g, _cover.color.b, _cover.color.a + Time.deltaTime * 1f);
            yield return null;
        }

        for (int i = 0; i < _victoryTexts.Length; i++)
        {
            while (_victoryTexts[i].color.a < 1f)
            {
                _victoryTexts[i].color = new Color(_victoryTexts[i].color.r, _victoryTexts[i].color.g, _victoryTexts[i].color.b, _victoryTexts[i].color.a + Time.deltaTime * 2f);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(4f);

        //reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
