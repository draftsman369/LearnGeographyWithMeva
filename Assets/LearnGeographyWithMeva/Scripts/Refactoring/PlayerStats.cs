using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("XP")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int xpGrowthPerLevel = 50;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Slider xpSlider;

    public static event EventHandler OnLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadStats();
        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateUI();
        SaveStats();
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel += xpGrowthPerLevel;

        Debug.Log("Level Up ! Niveau actuel : " + currentLevel);
        OnLevelUp?.Invoke(this, EventArgs.Empty);
    }

    public void SetUI(TextMeshProUGUI newLevelText, TextMeshProUGUI newXpText, Slider newXpSlider)
    {
        levelText = newLevelText;
        xpText = newXpText;
        xpSlider = newXpSlider;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Niveau " + currentLevel;

        if (xpText != null)
            xpText.text = currentXP + " / " + xpToNextLevel + " XP";

        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerXP", currentXP);
        PlayerPrefs.SetInt("PlayerXPToNextLevel", xpToNextLevel);
        PlayerPrefs.Save();
    }

    private void LoadStats()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetInt("PlayerXP", 0);
        xpToNextLevel = PlayerPrefs.GetInt("PlayerXPToNextLevel", 100);
    }
}