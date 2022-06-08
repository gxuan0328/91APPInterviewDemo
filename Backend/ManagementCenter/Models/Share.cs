using System.ComponentModel.DataAnnotations;
using static Error;

namespace ManagementCenter.Models
{
    public class ResponseFormat<T>
    {
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        /// <value></value>
        public Code StatusCode { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        /// <value></value>
        public string Message { get; set; }
        /// <summary>
        /// 資料
        /// </summary>
        /// <value></value>
        public T Data { get; set; }
    }
}





