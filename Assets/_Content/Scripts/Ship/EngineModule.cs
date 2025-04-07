using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// Module controlling the ship's movement.
/// </summary>
[Serializable]
public class EngineModule : ShipModule
{
	#region Fields
	/// <summary>
	/// The movement controller for the ship.
	/// </summary>
	public ShipLocomotion Locomotion;
	/// <summary>
	/// Reference to the head of the ship for telemetry.
	/// </summary>
	public Transform ShipHead;
	/// <summary>
	/// Throttle controller. Cannot be negative, bounded 0-1
	/// </summary>
	public Slider Throttle;
	/// <summary>
	/// Heading/steering controller, +/-20 degrees
	/// </summary>
	public Slider Heading;

	[SerializeField]
	protected float _startingMaximumSpeed; // m/s

	// Not shown in inspector
	protected Vector3 _previousPosition = Vector3.zero;
	#endregion Fields

	#region Properties
	/// <summary>
	/// Current maximum allowed speed of ship (m/s)
	/// </summary>
	public float MaximumSpeed
	{
		get
		{ 
			if (!IsActive)
			{
				return 0f;
			}

			return GetModifiedValue(ModifierStatType.ModuleEfficiency, CoreFunctionEfficiency * _startingMaximumSpeed);
		}
	}
	/// <summary>
	/// Throttle-set speed target (m/s)
	/// </summary>
	public float TargetSpeed { get; protected set; }
	/// <summary>
	/// Current speed of ship (m/s)
	/// </summary>
	public float CurrentSpeed { get; protected set; }
	/// <summary>
	/// Directional heading of ship (deg)
	/// </summary>
	public float CurrentHeading { get; protected set; }
	/// <summary>
	/// Target heading of ship (deg)
	/// </summary>
	public float TargetHeading { get; protected set; }
	#endregion Properties

	#region Methods
	private void UpdateHeading()
	{
		Locomotion.TargetSpeed = TargetSpeed = MaximumSpeed * Throttle.value;
		Locomotion.TargetRotation = TargetHeading = Heading.value;

		CurrentSpeed = (ShipHead.transform.position - _previousPosition).magnitude / Time.fixedDeltaTime;
		CurrentHeading = ShipHead.transform.eulerAngles.z;
		_previousPosition = ShipHead.transform.position;
	}
	#endregion Methods

	#region Events
	void FixedUpdate()
	{
		UpdateUI();
		if (IsActive)
		{
			UpdateHeading();
		}
		else
		{
			ModuleIdle();
		}
	}

	void Start()
	{
		ShipSystemsManager.Instance.Callback(this);
	}
	#endregion Events
}
