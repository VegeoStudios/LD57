using System.Collections;
using TMPro;
using UnityEngine;

public class FathomTrackerAudio : MonoBehaviour
{
	public AudioSource SoundSource;
	public AudioSource AmbienceSource;
	public TextMeshProUGUI FathomNameText;

	private int _fathomCount = 0;

	// Update is called once per frame
	void FixedUpdate()
	{
		if (ShipSystemsManager.Instance.Depth > 0)
		{
			if (ShipSystemsManager.Instance.FathomCount > _fathomCount)
			{
				SoundSource.Play();
				_fathomCount = ShipSystemsManager.Instance.FathomCount;
				StartCoroutine(ShowFathomMessage());
			}

			if (!AmbienceSource.isPlaying)
			{
				AmbienceSource.Play();
			}
		}
	}

	IEnumerator ShowFathomMessage()
	{
		FathomNameText.text = "Fathom " + ShipSystemsManager.Instance.FathomCount + " | " + ShipSystemsManager.Instance.CurrentTierName;
		yield return new WaitForSeconds(5f);
	}
}
