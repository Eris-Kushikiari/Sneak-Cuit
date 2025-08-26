using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectebleManager : MonoBehaviour
{
    public float biscuitCount;
    public TextMeshProUGUI biscuitText;
    public Image biscuitImage;

    void Update()
    {
        biscuitText.text = biscuitCount.ToString();
    }
}
