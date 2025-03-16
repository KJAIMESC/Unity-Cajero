using UnityEngine;

public class PreferenceSetter : MonoBehaviour
{
     public static void SetPreference(string key, object value)
    {
        if (value is int intValue)
        {
            PlayerPrefs.SetInt(key, intValue);
            Debug.Log($"🔹 Saved Int: {key} = {intValue}");
        }
        else if (value is float floatValue)
        {
            PlayerPrefs.SetFloat(key, floatValue);
            Debug.Log($"🔹 Saved Float: {key} = {floatValue}");
        }
        else if (value is string stringValue)
        {
            PlayerPrefs.SetString(key, stringValue);
            Debug.Log($"🔹 Saved String: {key} = {stringValue}");
        }
        else
        {
            Debug.LogError($"❌ Unsupported type for key: {key}");
            return;
        }

        PlayerPrefs.Save(); // Ensure values are saved
    }

    public static object GetPreference(string key, object defaultValue)
    {
        if (defaultValue is int)
        {
            return PlayerPrefs.GetInt(key, (int)defaultValue);
        }
        if (defaultValue is float)
        {
            return PlayerPrefs.GetFloat(key, (float)defaultValue);
        }
        if (defaultValue is string)
        {
            return PlayerPrefs.GetString(key, (string)defaultValue);
        }

        Debug.LogError($"❌ Unsupported default type for key: {key}");
        return null;
    }
}
