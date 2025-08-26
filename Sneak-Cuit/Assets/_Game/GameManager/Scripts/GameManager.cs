using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float riskLevel = 0f;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateRisk(float amount)
    {
        riskLevel = Mathf.Clamp(riskLevel + amount, 0, 1);
        GameUIManager.Instance.UpdateRiskSlider(riskLevel);
    }
}
