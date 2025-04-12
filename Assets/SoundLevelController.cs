using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Controls sound slider levels
/// </summary>
public class SoundLevelController : MonoBehaviour
{
	public Slider masterSlider;
	public Slider musicSlider;
	public Slider soundSlider;
	public Slider otherSlider;
	public AudioMixer mixer;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		masterSlider.value = PlayerPrefs.GetFloat("Master", 0.3039062f);
		SetMasterVolume();

        musicSlider.value = PlayerPrefs.GetFloat("Music", 0.3039062f);
        SetMusicVolume();

        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0.3039062f);
        SetEffectVolume();

        otherSlider.value = PlayerPrefs.GetFloat("Other", 0.3039062f);
        SetAmbienceVolume();
    }

	public void SetMasterVolume()
	{
		mixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        PlayerPrefs.SetFloat("Master", masterSlider.value);
    }

	public void SetMusicVolume()
	{
		mixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }

	public void SetEffectVolume()
	{
		mixer.SetFloat("Sounds", Mathf.Log10(soundSlider.value) * 20);
        PlayerPrefs.SetFloat("Sounds", soundSlider.value);
    }

	public void SetAmbienceVolume()
	{
		mixer.SetFloat("Other", Mathf.Log10(otherSlider.value) * 20);
        PlayerPrefs.SetFloat("Other", otherSlider.value);
    }
}