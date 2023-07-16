using System;
using System.Collections.Generic;

/// <summary>
/// ログイン系API.
/// </summary>
public class LoginApi
{
    /// <summary>
    /// user/loginのレスポンスデータ
    /// </summary>
    public class LoginResponseData
    {
        public bool loginSuccess;
        public LoginUserData tAuth;
    }

    /// <summary>
    /// ログイン時に取得できるのIDとhashのレスポンスデータ
    /// </summary>
    public class LoginUserData
    {
        public int userId;
        public string hash;
    }
    
    /// <summary>
    /// ログイン通信
    /// </summary>
    public static void CallLoginApi(UserData userData, Action onCompleted)
    {
        // リクエスト
        var request = new RASWebRequest<LoginResponseData>("user/login");
        request.SetRequestParameter(new Dictionary<string, object>
        {
            { "userId",     userData.userId },
            { "password",   userData.password },
            { "authType",   3 }, // TODO.
            { "deviceType", RASDefine.DEVICE_TYPE },
        });
        
        // 通信成功時、コールバック
        request.onSuccess = (response) =>
        {
            // ユーザー認証トークンセット
            userData.Set(response.tAuth);
            userData.Save();

            // 通信完了
            onCompleted?.Invoke();
        };

        // 通信開始
        request.Send();
    }
}
