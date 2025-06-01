using System;

[Serializable]
public class UserCreate
{
    public string email;
    public string password;
    public int work_time = 25;
    public int rest_time = 5;
}

[Serializable]
public class UserLogin
{
    public string email;
    public string password;
}

[Serializable]
public class UserResponse
{
    public int id;
    public string hash;
    public string email;
    public int work_time;
    public int rest_time;
    public string created_at;
}

[Serializable]
public class Token
{
    public string access_token;
    public string token_type;
}

[Serializable]
public class ActivityCreate
{
    public string name;
}

[Serializable]
public class ActivityResponse
{
    public int id;
    public string hash;
    public string name;
    public string started_at;
    public string finished_at;
    public int user_id;
    public string created_at;
}

[Serializable]
public class ActivityStart
{
    public string activity_hash;
}

[Serializable]
public class ActivityEnd
{
    public string activity_hash;
}

[Serializable]
public class ActivityAnalysisResponse
{
    public string analysis;
}

[Serializable]
public class ActivityArrayWrapper
{
    public ActivityResponse[] activities;
}