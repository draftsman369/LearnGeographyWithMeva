using UnityEngine;

public class AchievementSystem : MonoBehaviour
{

    public  static AchievementSystem Instance;
    private int experience;
    public int Experience
    {
        get { return experience; }
        set
        {
            experience = value;
            CheckForLevelUp();
        }
    }
    private int level;
    
    private void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }


    private void CheckForLevelUp()
    {
        int requiredExperience = level * 100; // Example: 100 XP per level
        if (experience >= requiredExperience)
        {
            level++;
            experience -= requiredExperience;
            Debug.Log("Level Up! Current Level: " + level);
        }
    }
  


    // Update is called once per frame
    void Update()
    {
        
    }
}
