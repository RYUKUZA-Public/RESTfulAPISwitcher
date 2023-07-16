using UnityEngine;

/// <summary>
/// ユーザーデータ
/// </summary>
public class UserData
{
    private static UserData instance = null;

    /// <summary>
    /// ユーザーID
    /// </summary>
    public int userId;
    /// <summary>
    /// ハッシュ
    /// </summary>
    public string loginHash;
    /// <summary>
    /// パスワード
    /// </summary>
    public string password;
    /// <summary>
    /// 名前
    /// </summary>
    public string name;
    /// <summary>
    /// レベル
    /// </summary>
    public uint lv;
    /// <summary>
    /// EXP
    /// </summary>
    public uint exp;
    /// <summary>
    /// コイン
    /// </summary>
    public ulong coin;
    /// <summary>
    /// 無償ジェム
    /// </summary>
    public ulong freeGem;
    /// <summary>
    /// 有償ジェム
    /// </summary>
    public ulong chargeGem;

    /// <summary>
    /// データ取得
    /// </summary>
    public static UserData Get()
    {
        return instance;
    }

    /// <summary>
    /// データ設定：ログイン時に
    /// </summary>
    public static void Set(UserData userData)
    {
        instance = userData;
    }

    /// <summary>
    /// ログインに必要な情報の端末への保存
    /// </summary>
    public void Save()
    {
#if !RAS_OFFLINE
        PlayerPrefs.SetInt("userId", this.userId);
        PlayerPrefs.SetString("hash", this.loginHash);
        PlayerPrefs.SetString("password", this.password);
#endif
    }

    /// <summary>
    /// ログインに必要な情報の端末からの読込
    /// </summary>
    public void Load()
    {
        if (!PlayerPrefs.HasKey("userId"))   return;
        if (!PlayerPrefs.HasKey("hash"))     return;
        if (!PlayerPrefs.HasKey("password")) return;

        this.userId = PlayerPrefs.GetInt("userId");
        this.loginHash = PlayerPrefs.GetString("hash");
        this.password = PlayerPrefs.GetString("password");
    }

#if UNITY_EDITOR
    /// <summary>
    /// ユーザーデータ削除
    /// </summary>
    [UnityEditor.MenuItem("Tools/Delete UserData")]
    private static void DeleteUserData()
    {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("hash");
        PlayerPrefs.DeleteKey("password");
    }
#endif

    /// <summary>
    /// サーバーの情報で更新
    /// </summary>
    public void Set(TUsers tUsers)
    {
        this.name = tUsers.userName;
        this.lv = tUsers.level;
        this.coin = tUsers.coin;
        this.freeGem = tUsers.freeGem;
        this.chargeGem = tUsers.chargeGem;
    }

    /// <summary>
    /// ログイン時のuserIDとhashをサーバーの情報で更新
    /// </summary>
    public void Set(LoginApi.LoginUserData loginUserData)
    {
        this.userId = loginUserData.userId;
        this.loginHash = loginUserData.hash;
    }

    /// <summary>
    /// ログイン時のユーザーデータを更新
    /// </summary>
    public void Set(FirstApi.FirstUserResponseData firstUserData)
    {
        Set(firstUserData.tUsers);
        //itemData = firstUserData.tItem;
    }
    
    /// <summary>
    /// 言語取得
    /// </summary>
    public static Language GetLanguage()
    {
#if LANGUAGE_ZH
        Language language = Language.Zh;
#elif LANGUAGE_TW
        Language language = Language.Tw;
#elif LANGUAGE_EN
        Language language = Language.En;
#else
        Language language = Language.Ja;
#endif
        if (PlayerPrefs.HasKey("language"))
        {
            language = (Language)PlayerPrefs.GetInt("language");
        }
        else
        {
            PlayerPrefs.SetInt("language", (int)language);
        }

        return language;
    }
}
