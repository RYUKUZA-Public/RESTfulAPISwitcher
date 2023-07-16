/// <summary>
/// 共通定義
/// </summary>
public static class RASDefine
{
    public const int SCREEN_WIDTH = 1920;
    public const int SCREEN_HEIGHT = 1080;

#if UNITY_EDITOR
    public const int DEVICE_TYPE = 3;
#elif UNITY_IOS
    public const int DEVICE_TYPE = 2;
#else
    public const int DEVICE_TYPE = 3;
#endif
}

/// <summary>
/// 言語
/// </summary>
public enum Language
{
    Ja,
    Zh,
    Tw,
    En,
}