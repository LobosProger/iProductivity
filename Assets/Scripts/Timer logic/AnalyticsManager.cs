using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async UniTask GetAiAnalysisAsync()
    {
        Debug.Log("Requesting AI analysis from server...");
        
        var responseText = await ApiClient.instance.GetRequestAsync("/api/v1/activities/ai-analysis");
        
        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var analysisResponse = JsonUtility.FromJson<ActivityAnalysisResponse>(responseText);
                Debug.Log($"AI Analysis received: {analysisResponse.analysis}");
                DisplayAnalysisInConsole(analysisResponse.analysis);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse AI analysis response: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("Failed to get AI analysis from server");
        }
    }

    private void DisplayAnalysisInConsole(string analysis)
    {
        Debug.Log(analysis);
    }
}