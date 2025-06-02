# 🍅 Pomodorka - Smart Productivity Timer

![Unity](https://img.shields.io/badge/Unity-2022.3+-black?style=flat-square&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Mobile%20%7C%20Desktop-blue?style=flat-square)

A modern Unity-based productivity application implementing the Pomodoro Technique with AI-powered analytics and cloud synchronization. Designed to help users overcome procrastination and manage time more effectively through scientifically-backed work and rest intervals.

## ✨ Features

**Core Functionality:**
- 🎯 **Activity Management** - Create, organize and track your work activities
- ⏱️ **Smart Timer** - Automatic alternation between 25-minute work periods and 5-minute breaks
- 🔄 **Flexible Sessions** - Customizable session durations with circular slider interface
- 📊 **Time Analytics** - Detailed tracking of completed sessions and productivity metrics
- 🤖 **AI Analysis** - Personalized productivity insights and recommendations
- ☁️ **Cloud Sync** - Seamless data synchronization across devices

**User Experience:**
- 🎨 **Clean UI** - Intuitive interface with smooth animations
- 📱 **Cross-Platform** - Optimized for both mobile and desktop
- 🔐 **Secure Auth** - User registration and login system
- 🎵 **Audio Feedback** - Session completion notifications

## 🛠️ Tech Stack

### Unity Engine & Packages
- **Unity 2022.3+** - Core engine
- **UniTask** - Async/await operations and performance optimization
- **DOTween** - Smooth UI animations and transitions
- **TextMeshPro** - Advanced text rendering
- **Unity UI (uGUI)** - User interface system

### Backend Integration
- **REST API** - Full CRUD operations for activities and user management
- **JWT Authentication** - Secure token-based authorization
- **AI Analytics Endpoint** - Server-side productivity analysis

### Architecture Patterns
- **Singleton Pattern** - Core managers (UserManager, ActivityManager, ApiClient)
- **Event-Driven Architecture** - Decoupled component communication
- **MVC Pattern** - Clean separation of UI, logic, and data

## 🏛️ Architecture Excellence

This project showcases **professional-grade Unity architecture** with enterprise-level code organization and design patterns:

### 🎯 **Modular Design & Separation of Concerns**
The codebase demonstrates exceptional modularity with clear boundaries between different system layers:
- **Backend Logic** - Isolated API communication with proper error handling
- **Timer Logic** - Self-contained Pomodoro session management
- **UI Utilities** - Reusable components for consistent user experience
- **General Systems** - Core application utilities and configurations

### 🔄 **Event-Driven Architecture**
Implements a robust event system that eliminates tight coupling between components:
```csharp
// Clean event communication without direct dependencies
public static event Action<string> onSelectedActivity;
public event Action onSessionOfTimerStarted;
public event Action onSessionOfTimerEnded;
```

### 🏭 **Singleton Pattern Implementation**
Core managers use thread-safe singleton pattern with proper lifecycle management:
```csharp
public static ActivityManager instance { get; private set; }

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
```

### ⚡ **Async/Await with UniTask**
Modern asynchronous programming eliminates blocking operations and improves performance:
```csharp
public async UniTask<bool> LoginUserAsync(string email, string password)
{
    var responseText = await ApiClient.instance.PostRequestAsync("/api/v1/users/login", jsonData);
    // Non-blocking API calls with proper error handling
}
```

### 🎭 **Window Management System**
Sophisticated UI navigation with state management and smooth transitions:
```csharp
public static void ClosePreviousAndShowThisWindow(WindowView windowForShow)
{
    ClosePreviousWindow();
    windowForShow.canvasGroup.DOFade(1, k_transitionDuration);
    _previousOpenedWindows.Push(windowForShow);
}
```

### 🔧 **Dependency Injection Ready**
Components are designed with loose coupling, making them perfect for DI frameworks:
- Interface-based design (`IWindowView`)
- Constructor injection support
- Service locator pattern implementation

### 📦 **SOLID Principles Adherence**
- **Single Responsibility** - Each class has one clear purpose
- **Open/Closed** - Extensible through inheritance (`ExtendedToggle`, `MultiImageButton`)
- **Liskov Substitution** - Proper inheritance hierarchies
- **Interface Segregation** - Focused interfaces like `IWindowView`
- **Dependency Inversion** - High-level modules don't depend on low-level details

## 📁 Project Structure

```
Assets/Scripts/
├── Backend logic/           # API integration and data models
│   ├── ApiClient.cs        # HTTP client with authentication
│   ├── UserManager.cs      # User authentication and profile
│   ├── ActivityManager.cs  # Activity CRUD operations
│   └── ApiModels.cs        # Data transfer objects
├── Timer logic/            # Core Pomodoro functionality
│   ├── SessionManager.cs   # Session orchestration and state
│   ├── TimerController.cs  # Timer implementation with UniTask
│   ├── ActivitiesSetter.cs # Activity selection and management UI
│   └── SessionConfig.cs    # Pomodoro technique constants
├── UI utilities/           # Reusable UI components
│   ├── WindowView.cs       # Modal window system with DOTween
│   ├── ExtendedToggle.cs   # Enhanced toggle with events
│   └── MultiImageButton.cs # Multi-graphic button component
└── General/                # Core application utilities
    └── FPSSetter.cs        # Performance optimization
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Git LFS (for large assets)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/pomodorka.git
   cd pomodorka
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open with Unity 2022.3+

3. **Install Dependencies**
   - UniTask will be automatically installed via Package Manager
   - Ensure DOTween is imported (if using DOTween Pro)
   - Import NaughtyAttributes for inspector enhancements

4. **Configure API**
   - Update `k_baseUrl` in `ApiClient.cs` with your backend URL
   - Default: `https://pomodorka-api.vercel.app`

## 🔧 Configuration

### Session Settings
Modify `SessionConfig.cs` to customize Pomodoro intervals:
```csharp
public const int k_workIntervalMinutes = 25;    // Work period
public const int k_breakIntervalMinutes = 5;    // Break period
public const int k_cycleDurationMinutes = 30;   // Full cycle
```

### API Configuration
Update backend URL in `ApiClient.cs`:
```csharp
private const string k_baseUrl = "https://your-api-url.com";
```

## 🎮 Usage

1. **Registration/Login** - Create account or sign in
2. **Create Activities** - Add work tasks you want to track
3. **Select Activity** - Choose current task from your list
4. **Set Timer** - Use circular slider to set session duration
5. **Start Session** - Begin focused work period
6. **Take Breaks** - Automatic break intervals between work sessions
7. **View Analytics** - Get AI insights on your productivity patterns