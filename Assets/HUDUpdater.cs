using TMPro;
using UnityEngine;

public class HUDUpdater : MonoBehaviour
{
	public TextMeshProUGUI TimeHeatLoseGame;
	public TextMeshProUGUI ExtTemp;
	public TextMeshProUGUI IntTemp;
	public TextMeshProUGUI AmbHeat;
	public TextMeshProUGUI SysHeat;
	public TextMeshProUGUI CoolLoad;
	public TextMeshProUGUI CoolLeft;
	public TextMeshProUGUI EstTimeCoolant;

	public TextMeshProUGUI Depth;
	public TextMeshProUGUI Speed;
	public TextMeshProUGUI TargetSpeed;
	public TextMeshProUGUI CurrentHeading;
	public TextMeshProUGUI TargetHeading;
	public TextMeshProUGUI PowerDemand;
	public TextMeshProUGUI CurrentProduction;
	public TextMeshProUGUI PowerCapacity;
	public TextMeshProUGUI Efficiency;
	public TextMeshProUGUI FuelLeft;
	public TextMeshProUGUI EstTimeFuel;

	Color normal = new Color(1f, 1f, 1f);
	Color alert = new Color(1f, 0f, 0f);

	private void Update()
	{
		var mgr = ShipSystemsManager.Instance;

		TimeHeatLoseGame.text = "Overheating Failsafe" + RoundValue(mgr.OverheatTimeRemaining) + " s";
		ExtTemp.text = "External Temperature" + RoundValue(mgr.ExternalTemperature) + " C";
		IntTemp.text = "Internal Temperature" + RoundValue(mgr.InternalTemperature) + " C";
		AmbHeat.text = "Ambient Heat" + RoundValue(mgr.AmbientHeatInflux) + " kWt";
		SysHeat.text = "System Heat" + RoundValue(mgr.TotalSystemsHeat) + " kWt";
		CoolLoad.text = "Cooling Load" + RoundValue(mgr.TotalCoolingLoad) + " kWt";
		CoolLeft.text = "Coolant Remaining" + RoundValue(mgr.CoolantRemaining) + " kWh";
		EstTimeCoolant.text = "Est. Time Remaining" + RoundValue((float)mgr.EstimatedCoolingDuration.TotalSeconds) + " s";

		Depth.text = "Depth" + RoundValue(mgr.Depth) + " m";
		Speed.text = "Current Speed" + RoundValue(mgr.CurrentSpeed) + " m/s";
		TargetSpeed.text = "Target Speed" + RoundValue(mgr.TargetSpeed) + " m/s";

		float heading = mgr.CurrentHeading > 30f ? mgr.CurrentHeading - 360f : mgr.CurrentHeading;

		CurrentHeading.text = "Current Heading" + RoundValue(heading) + " deg";
		TargetHeading.text = "Target Heading" + RoundValue(mgr.TargetHeading) + " deg";

		PowerDemand.text = "Power Demand" + RoundValue(mgr.TotalPowerDemand) + " kWe";
		CurrentProduction.text = "Power Production" + RoundValue(mgr.TotalPowerProduction) + " kWe";
		PowerCapacity.text = "Power Capacity" + RoundValue(mgr.ReactorModule.MaximumPowerProduction) + " kWe";
		Efficiency.text = "Operational Efficiency" + RoundValue(mgr.OperationalEfficiency * 100f) + " %";
		FuelLeft.text = "Fuel Remaining" + RoundValue(mgr.FuelRemaining) + " kWh";
		EstTimeFuel.text = "Est. Time Remaining" + RoundValue((float)mgr.EstimatedPowerDuration.TotalSeconds) + " s";

		Color powerSys = mgr.PowerAlert ? alert : normal;
		PowerDemand.color = powerSys;
		Efficiency.color = powerSys;

		Color heatSys = mgr.HeatAlert ? alert : normal;
		IntTemp.color = heatSys;
		TimeHeatLoseGame.color = heatSys;
	}

	public string RoundValue(float val)
	{
		return "\n" + val.ToString("0.00");
	}
}
