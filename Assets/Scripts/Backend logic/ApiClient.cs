using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    public static ApiClient instance { get; private set; }
    
    private const string k_baseUrl = "https://pomodorka-api.vercel.app"; // Replace with your actual API URL
    private string _authToken;

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

    private void Update()
    {
        Debug.Log(_authToken);
    }

    public void SetAuthToken(string token)
    {
        _authToken = token;
    }

    public async UniTask<string> GetRequestAsync(string endpoint)
    {
        var url = k_baseUrl + endpoint;
        
        using var request = UnityWebRequest.Get(url);
        AddAuthHeader(request);
        
        try
        {
            await request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"GET request failed: {request.error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"GET request failed: {ex.Message}");
            return null;
        }
    }

    public async UniTask<string> PostRequestAsync(string endpoint, string jsonData)
    {
        var url = k_baseUrl + endpoint;
        
        using var request = new UnityWebRequest(url, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        AddAuthHeader(request);
        
        try
        {
            await request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"POST request failed: {request.error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"POST request failed: {ex.Message}");
            return null;
        }
    }

    public async UniTask<string> PutRequestAsync(string endpoint, string jsonData = null)
    {
        var url = k_baseUrl + endpoint;
        
        using var request = new UnityWebRequest(url, "PUT");
        
        if (!string.IsNullOrEmpty(jsonData))
        {
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }
        
        request.downloadHandler = new DownloadHandlerBuffer();
        AddAuthHeader(request);
        
        try
        {
            await request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"PUT request failed: {request.error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"PUT request failed: {ex.Message}");
            return null;
        }
    }

    public async UniTask<bool> DeleteRequestAsync(string endpoint)
    {
        var url = k_baseUrl + endpoint;
        
        using var request = UnityWebRequest.Delete(url);
        AddAuthHeader(request);
        
        try
        {
            await request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                Debug.LogError($"DELETE request failed: {request.error}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"DELETE request failed: {ex.Message}");
            return false;
        }
    }

    private void AddAuthHeader(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(_authToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
        }
    }
}