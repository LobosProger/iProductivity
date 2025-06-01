using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager instance { get; private set; }
    
    public event Action<UserResponse> onUserLoggedIn;
    public event Action onUserLoggedOut;
    public event Action<string> onLoginFailed;
    public event Action<string> onRegistrationFailed;

    private UserResponse _currentUser;
    private bool _isLoggedIn;

    private void Awake()
    {
        instance = this;
    }

    public async UniTask<bool> RegisterUserAsync(string email, string password, int workTime = 25, int restTime = 5)
    {
        var registerData = new UserCreate
        {
            email = email,
            password = password,
            work_time = workTime,
            rest_time = restTime
        };

        var jsonData = JsonUtility.ToJson(registerData);
        var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/users/register", jsonData);

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                _currentUser = JsonUtility.FromJson<UserResponse>(responseText);
                Debug.Log("User registered successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse registration response: {ex.Message}");
                onRegistrationFailed?.Invoke("Failed to parse server response");
                return false;
            }
        }
        else
        {
            onRegistrationFailed?.Invoke("Registration failed");
            return false;
        }
    }

    public async UniTask<bool> LoginUserAsync(string email, string password)
    {
        var loginData = new UserLogin
        {
            email = email,
            password = password
        };

        var jsonData = JsonUtility.ToJson(loginData);
        var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/users/login", jsonData);

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                var tokenResponse = JsonUtility.FromJson<Token>(responseText);
                ApiClient.instance.SetAuthToken(tokenResponse.access_token);
                
                // Get user profile after successful login
                var profileSuccess = await GetCurrentUserProfileAsync();
                if (profileSuccess)
                {
                    _isLoggedIn = true;
                    onUserLoggedIn?.Invoke(_currentUser);
                    Debug.Log($"User logged in successfully: {_currentUser.email}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse login response: {ex.Message}");
                onLoginFailed?.Invoke("Failed to parse server response");
                return false;
            }
        }
        
        onLoginFailed?.Invoke("Login failed");
        return false;
    }

    public async UniTask<bool> GetCurrentUserProfileAsync()
    {
        var responseText = await ApiClient.instance.GetRequestAsync("/api/v1/users/me");

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                _currentUser = JsonUtility.FromJson<UserResponse>(responseText);
                Debug.Log($"User profile loaded: {_currentUser.email}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse user profile: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to get user profile");
            return false;
        }
    }

    public async UniTask<bool> UpdateUserProfileAsync(int? workTime = null, int? restTime = null)
    {
        var queryParams = "";
        if (workTime.HasValue)
        {
            queryParams += $"?work_time={workTime.Value}";
        }
        if (restTime.HasValue)
        {
            queryParams += string.IsNullOrEmpty(queryParams) ? $"?rest_time={restTime.Value}" : $"&rest_time={restTime.Value}";
        }

        var responseText = await ApiClient.instance.PutRequestAsync($"/api/v1/users/me{queryParams}");

        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                _currentUser = JsonUtility.FromJson<UserResponse>(responseText);
                Debug.Log("User profile updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse update response: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to update user profile");
            return false;
        }
    }

    public void LogoutUser()
    {
        _isLoggedIn = false;
        _currentUser = null;
        ApiClient.instance.SetAuthToken(null);
        onUserLoggedOut?.Invoke();
        Debug.Log("User logged out");
    }

    public bool IsLoggedIn()
    {
        return _isLoggedIn && _currentUser != null;
    }

    public UserResponse GetCurrentUser()
    {
        return _currentUser;
    }
}