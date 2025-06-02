using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsFetcher : MonoBehaviour
{
    [SerializeField] private LayoutGroupFix _layoutGroupFix;
    [SerializeField] private TMP_Text _analyticsText;
    [SerializeField] private Button _fetchAnalyticsButton;

    private void Start()
    {
        _fetchAnalyticsButton.onClick.AddListener(FetchAnalyticsAndShowAtUI);
    }

    private void FetchAnalyticsAndShowAtUI()
    {
        FetchAnalyticsAndShowAtUIAsync().Forget();
    }

    private async UniTask FetchAnalyticsAndShowAtUIAsync()
    {
        _fetchAnalyticsButton.interactable = false;
        var analyticsText = await GetAiAnalysisAsync();

        if (string.IsNullOrEmpty(analyticsText))
        {
            _analyticsText.text = "Error of fetching analytics, try again later.";
        }
        else
        {
            _analyticsText.text = analyticsText;
        }
        
        _layoutGroupFix.FixLayout();
    }

    private async UniTask<string> GetAiAnalysisAsync()
    {
        Debug.Log("Requesting AI analysis from server...");
        
        var responseText = await ApiClient.instance.GetRequestAsync("/api/v1/activities/ai-analysis");
        
        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var analysisResponse = JsonUtility.FromJson<ActivityAnalysisResponse>(responseText);
                Debug.Log($"AI Analysis received: {analysisResponse.analysis}");
                
                return analysisResponse.analysis;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse AI analysis response: {ex.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogError("Failed to get AI analysis from server");
            return null;
        }
    }
}