using UnityEngine;

public class ModuleLoopSound : MonoBehaviour
{
	private AudioSource SoundSource;
	private ShipModule ShipModule;

	private void Start()
	{
		SoundSource = GetComponent<AudioSource>();
		ShipModule = GetComponent<ShipModule>();
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        if (ShipModule.IsActive)
		{
			if (ShipModule is EngineModule engineModule)
			{
				SoundSource.pitch = Mathf.Clamp(engineModule.CurrentSpeed / engineModule.MaximumSpeed, 0f, 1f);
			}
			else
			{
				SoundSource.pitch = Mathf.Clamp(ShipModule.OperationalEfficiency, 0.25f, 1f);
			}

			if (SoundSource.pitch < 0.02f)
			{
				SoundSource.Stop();
			}
			else if (!SoundSource.isPlaying)
			{
				SoundSource.Play();
			}
		}
		else
		{
			SoundSource.Stop();
		}
    }
}
