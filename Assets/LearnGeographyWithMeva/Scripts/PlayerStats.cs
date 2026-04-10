using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.Interactions;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    InputAction addExperienceAction;

    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;


    private int currentLevel, totalExperience;
    private int previousLevelExperience, nextLevelExperience;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private Image experienceFill;

    private void OnEnable()
    {
        addExperienceAction = new InputAction("AddExperience", binding: "<Keyboard>/e");
        addExperienceAction.performed += ctx => AddExperience(10);
        addExperienceAction.Enable();
    }

    private void OnDisable()
    {
        addExperienceAction.Disable();
    }

    private void Start()
    {
        currentLevel = 1;
        totalExperience = 0;
        UpdateLevel();
    }
    private void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    private void CheckForLevelUp()
    {
        if(totalExperience >= nextLevelExperience)
        {
            currentLevel++;
            UpdateLevel();
            //start level up sequence... vfx or sound
        }
    }

    private void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
    }

    private void UpdateInterface()
    {
        int start = totalExperience - previousLevelExperience;
        int end = nextLevelExperience - previousLevelExperience;

        levelText.text = $"{currentLevel}";

        experienceText.text = $"{totalExperience} exp/{nextLevelExperience} exp";
        experienceFill.fillAmount = (float)start / end;
    }

}
