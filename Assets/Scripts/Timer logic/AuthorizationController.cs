using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthorizationController : MonoBehaviour
{
    [SerializeField] private WindowView _mainMenuWindow;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [Space]
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
    [SerializeField] private TMP_Text _authStatusText;

    private void Start()
    {
        _loginButton.onClick.AddListener(LoginUser);
        _registerButton.onClick.AddListener(RegisterUser);
        
        // Subscribe to user manager events
        UserManager.instance.onUserLoggedIn += OnUserLoggedIn;
        UserManager.instance.onLoginFailed += OnLoginFailed;
        UserManager.instance.onRegistrationFailed += OnRegistrationFailed;
    }

    private void OnDestroy()
    {
        if (UserManager.instance != null)
        {
            UserManager.instance.onUserLoggedIn -= OnUserLoggedIn;
            UserManager.instance.onLoginFailed -= OnLoginFailed;
            UserManager.instance.onRegistrationFailed -= OnRegistrationFailed;
        }
    }

    private async void LoginUser()
    {
        var email = _emailInputField.text;
        var password = _passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            UpdateAuthStatus("Please fill in all fields");
            return;
        }

        UpdateAuthStatus("Logging in...");
        SetButtonsInteractable(false);

        var success = await UserManager.instance.LoginUserAsync(email, password);
        
        if (!success)
        {
            SetButtonsInteractable(true);
        }
    }

    private async void RegisterUser()
    {
        var email = _emailInputField.text;
        var password = _passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            UpdateAuthStatus("Please fill in all fields");
            return;
        }

        UpdateAuthStatus("Registering...");
        SetButtonsInteractable(false);

        var success = await UserManager.instance.RegisterUserAsync(email, password);
        
        if (success)
        {
            //UpdateAuthStatus("Registration successful! Please login.");
            _loginButton.onClick.Invoke();
        }
        else
        {
            SetButtonsInteractable(true);
        }
    }

    private void OnUserLoggedIn(UserResponse user)
    {
        UpdateAuthStatus($"Welcome, {user.email}!");
        
        // Here you can navigate to main menu or load activities
        LoadUserActivitiesAfterLogin();
    }

    private void OnLoginFailed(string error)
    {
        UpdateAuthStatus($"Login failed: {error}");
    }

    private void OnRegistrationFailed(string error)
    {
        UpdateAuthStatus($"Registration failed: {error}");
    }

    private async void LoadUserActivitiesAfterLogin()
    {
        // Load user activities after successful login
        await ActivityManager.instance.LoadUserActivitiesAsync();
        
        _mainMenuWindow.ClosePreviousAndShowThisWindow();
        TopDownPanel.instance.ShowPanel();
    }

    private void UpdateAuthStatus(string message)
    {
        _authStatusText.text = message;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        _loginButton.interactable = interactable;
        _registerButton.interactable = interactable;
    }
}