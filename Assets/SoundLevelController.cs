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

		/*
		mixer.GetFloat("Master", out float masterVol);
		masterSlider.value = Mathf.Pow(10, masterVol / 20);

		mixer.GetFloat("Other", out float ambienceVol);
		otherSlider.value = Mathf.Pow(10, ambienceVol / 20);

		mixer.GetFloat("Sounds", out float soundsVol);
		soundSlider.value = Mathf.Pow(10, soundsVol / 20);

		mixer.GetFloat("Music", out float musicVol);
		musicSlider.value = Mathf.Pow(10, musicVol / 20);
		*/

		masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.3039062f);
		SetMasterVolume();

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.3039062f);
        SetMusicVolume();

        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.3039062f);
        SetEffectVolume();

        otherSlider.value = PlayerPrefs.GetFloat("OtherVolume", 0.3039062f);
        SetAmbienceVolume();
    }

	public void SetMasterVolume()
	{
		mixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

	public void SetMusicVolume()
	{
		mixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

	public void SetEffectVolume()
	{
		mixer.SetFloat("Sounds", Mathf.Log10(soundSlider.value) * 20);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
    }

	public void SetAmbienceVolume()
	{
		mixer.SetFloat("Other", Mathf.Log10(otherSlider.value) * 20);
        PlayerPrefs.SetFloat("OtherVolume", otherSlider.value);
    }
}