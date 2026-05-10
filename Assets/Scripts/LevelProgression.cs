using UnityEngine;

public static class LevelProgression
{
    public const string HighestLevelKey = "HighestLevel";
    public const int FirstLevel = 1;
    public const int LastConfiguredLevel = 3;

    public static void EnsureInitialized()
    {
        if (!PlayerPrefs.HasKey(HighestLevelKey) || PlayerPrefs.GetInt(HighestLevelKey, FirstLevel) < FirstLevel)
        {
            PlayerPrefs.SetInt(HighestLevelKey, FirstLevel);
            PlayerPrefs.Save();
        }
    }

    public static int GetHighestUnlockedLevel()
    {
        EnsureInitialized();
        return Mathf.Max(PlayerPrefs.GetInt(HighestLevelKey, FirstLevel), FirstLevel);
    }

    public static bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= GetHighestUnlockedLevel();
    }

    public static void CompleteLevel(int currentLevel)
    {
        UnlockLevel(currentLevel + 1);
    }

    public static void UnlockLevel(int levelNumber)
    {
        int normalizedLevel = Mathf.Max(levelNumber, FirstLevel);
        int highestUnlockedLevel = GetHighestUnlockedLevel();

        if (normalizedLevel > highestUnlockedLevel)
        {
            PlayerPrefs.SetInt(HighestLevelKey, normalizedLevel);
            PlayerPrefs.Save();
        }
    }
}
