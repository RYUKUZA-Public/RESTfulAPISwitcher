using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// マスター系API
/// </summary>
public class MasterApi
{
    /// <summary>
    /// マスターデータ取得通信
    /// </summary>
    public static void CallGetMasterApi(Action onCompleted, params Master.IDataBase[] dbs)
    {
#if RAS_OFFLINE
        onCompleted?.Invoke();
        return;
#endif
        var request = new RASWebRequest<Dictionary<string, object>>("master/getMaster");

        request.SetRequestHeader("AccessToken", UserData.Get().loginHash);

        request.SetRequestParameter(new Dictionary<string, object>
        {
            { "tableNameList", dbs.Select(x => x.tableName).ToArray() },
        });

        request.onSuccess = (response) =>
        {
            foreach (var db in dbs)
            {
                db.SetList(response[db.tableName].ToString());
            }

            onCompleted?.Invoke();
        };

        request.Send();
    }
}
