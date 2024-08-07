using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static Config LoadConfig()
    {
        TextAsset configFile = Resources.Load<TextAsset>("Azure/azureconfig");
        if (configFile != null)
        {
            return JsonUtility.FromJson<Config>(configFile.text);
        }
        Debug.LogError("Config file not found!");
        return null;
    }
}
