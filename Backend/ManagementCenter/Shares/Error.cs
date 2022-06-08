using System.Collections.Generic;

public class Error
{
    public enum Code
    {
        SUCCESS = 0,
        DATABASE_ERROR = -1,
        PERMISSION_DENIED = -2,
        BAD_REQUEST = -3,
        RESULT_NOT_FOUND = -4,
    }

    public static Dictionary<Code, string> Message = new Dictionary<Code, string>()
    {
        {Code.SUCCESS, "成功"},
        {Code.DATABASE_ERROR, "資料庫異常"}, // 資料庫異常
        {Code.PERMISSION_DENIED, "沒有權限"}, // 沒有權限
        {Code.BAD_REQUEST, "Request參數未填or有問題"}, // Request參數未填or有問題
        {Code.RESULT_NOT_FOUND, "查無結果"}, // 查無結果
    };
}