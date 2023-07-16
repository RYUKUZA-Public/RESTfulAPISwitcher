using System;
using UnityEngine;

/// <summary>
/// Login時、ユーザーデータ取得API
/// </summary>
public class FirstApi
{
    /// <summary>
    /// first/userのレスポンスデータ
    /// </summary>
    public class FirstUserResponseData
    {
        public TUsers tUsers;
    }

    /// <summary>
    /// ユーザーデータ取得通信 (Login時のみ)
    /// </summary>
    public static void CallFirstUserApi(UserData userData, Action onCompleted)
    {
        // リクエスト
        var request = new RASWebRequest<FirstUserResponseData>("first/user");
        // Token認証
        request.SetRequestHeader("AccessToken", UserData.Get().loginHash);

        // 通信成功時、コールバック
        request.onSuccess = (response) =>
        {
            // 通信で取得したデータを格納
            userData.Set(response);

            // 通信完了
            onCompleted?.Invoke();
        };

        request.onError = (errorCode) =>
        {
            Debug.LogFormat($"errorCode : {errorCode}");
        };

        // 通信開始
        request.Send();
    }
}
