using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShipLightingSystem : MonoBehaviour
{
    public Light2D[] Ambient;
    public Light2D[] ExternalClearance;
    public Light2D[] ExternalWarning;
    public Light2D[] PrimaryInternal;
    public Light2D[] SecondaryInternal;
    public Light2D[] Emergency;
    public Light2D[] Hall;

    private Color[] _ambientColors;
    private Color[] _externalClearanceColors;
    private Color[] _externalWarningColors;
    private Color[] _primaryInternalColors;
    private Color[] _secondaryInternalColors;
    private Color[] _emergencyColors;
    private Color[] _hallColors;

    private bool _lightsActive = true;

    private void Awake()
    {
        _ambientColors = new Color[Ambient.Length];
        _externalClearanceColors = new Color[ExternalClearance.Length];
        _externalWarningColors = new Color[ExternalWarning.Length];
        _primaryInternalColors = new Color[PrimaryInternal.Length];
        _secondaryInternalColors = new Color[SecondaryInternal.Length];
        _emergencyColors = new Color[Emergency.Length];
        _hallColors = new Color[Hall.Length];
        StoreInitialLightColors();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _lightsActive = !_lightsActive;
            SetAllActive(_lightsActive);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetAllLightColor(Color.red);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SetAllLightColor(Color.white);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            ResetLightColors();
        }
    }

    private void StoreInitialLightColors()
    {
        StoreLightColors(Ambient, _ambientColors);
        StoreLightColors(ExternalClearance, _externalClearanceColors);
        StoreLightColors(ExternalWarning, _externalWarningColors);
        StoreLightColors(PrimaryInternal, _primaryInternalColors);
        StoreLightColors(SecondaryInternal, _secondaryInternalColors);
        StoreLightColors(Emergency, _emergencyColors);
        StoreLightColors(Hall, _hallColors);
    }

    private void StoreLightColors(Light2D[] lights, Color[] colors)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            colors[i] = lights[i].color;
        }
    }

    private void ResetLightColors()
    {
        SetLightColors(Ambient, _ambientColors);
        SetLightColors(ExternalClearance, _externalClearanceColors);
        SetLightColors(ExternalWarning, _externalWarningColors);
        SetLightColors(PrimaryInternal, _primaryInternalColors);
        SetLightColors(SecondaryInternal, _secondaryInternalColors);
        SetLightColors(Emergency, _emergencyColors);
        SetLightColors(Hall, _hallColors);
    }

    private void SetLightColors(Light2D[] lights, Color[] colors)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].color = colors[i];
        }
    }

    private void SetAllLightColor(Color color)
    {
        SetLightColors(Ambient, color);
        SetLightColors(ExternalClearance, color);
        SetLightColors(ExternalWarning, color);
        SetLightColors(PrimaryInternal, color);
        SetLightColors(SecondaryInternal, color);
        SetLightColors(Emergency, color);
        SetLightColors(Hall, color);
    }

    private void SetLightColors(Light2D[] lights, Color color)
    {
        foreach (var light in lights)
        {
            light.color = color;
        }
    }

    public void SetAllActive(bool active)
    {
        SetLightsActive(Ambient, active);
        SetLightsActive(ExternalClearance, active);
        SetLightsActive(ExternalWarning, active);
        SetLightsActive(PrimaryInternal, active);
        SetLightsActive(SecondaryInternal, active);
        SetLightsActive(Emergency, active);
        SetLightsActive(Hall, active);
    }

    private void SetLightsActive(Light2D[] lights, bool active)
    {
        foreach (var light in lights)
        {
            light.enabled = active;
        }
    }
}
