using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI Elements")]
    public Slider riskSlider;


    private void Awake()
    {
        if (Instance == null)  Instance = this;
    }

    public void UpdateRiskSlider(float value)
    {
        riskSlider.value = value;
    }
}
