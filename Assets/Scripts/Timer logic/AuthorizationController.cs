using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthorizationController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [Space]
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _registerButton;
}
