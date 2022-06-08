using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace ManagementCenter.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private SqlFunction sqlFunction;
        private int userId;


        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
            sqlFunction = new SqlFunction(_configuration);
            //因目前未實作身分識別機制，暫以此模擬從Middleware解讀出來的使用者資訊
            userId = 1;
        }

        /// <summary>
        /// 查詢訂單GET:order/list
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public ResponseFormat<List<OrderOverviewDto>> GetOrderList()
        {
            ResponseFormat<List<OrderOverviewDto>> result = new ResponseFormat<List<OrderOverviewDto>>();
            List<OrderOverviewDto> overview = new List<OrderOverviewDto>();

            try
            {
                string sql = @"
                SELECT A.`Id`
                    ,F.`Name` AS Buyer
                    ,E.`Name` AS PaymentType
                    ,SUM(C.`Price` * B.`Quantity`) AS Total
                    ,A.`Address`
                    ,A.`CreateTime`
                    ,D.`Id` AS OrderStatusType_Id
                    ,D.`Name` AS OrderStatusType 
                FROM `ManagementCenter`.`Order` AS A
                INNER JOIN `ManagementCenter`.`Order_Product_Relation` AS B
                ON A.`Id` = B.`Order_Id`
                INNER JOIN `ManagementCenter`.`Product` AS C
                ON B.`Product_Id` = C.`Id`
                INNER JOIN `ManagementCenter`.`OrderStatusType` AS D
                ON A.`OrderStatusType_Id` = D.`Id`
                INNER JOIN `ManagementCenter`.`PaymentType` AS E
                ON A.`PaymentType_Id` = E.`Id`
                INNER JOIN `ManagementCenter`.`User` AS F
                ON A.`Buyer_Id` = F.`Id`
                WHERE A.`Seller_Id` = @Seller_Id
                GROUP BY A.`Id`";

                MySqlParameter[] parameters = new[]
                {
                    new MySqlParameter("@Seller_Id", MySqlDbType.Int32) { Value = userId }
                };

                DataTable dtData = sqlFunction.GetData(sql, parameters);

                overview = sqlFunction.DataTableToList<OrderOverviewDto>(dtData);
            }
            catch (Exception e)
            {
                result.StatusCode = Error.Code.DATABASE_ERROR;
                result.Message = e.Message;
                return result;
            }

            result.StatusCode = Error.Code.SUCCESS;
            result.Message = Error.Message[Error.Code.SUCCESS];
            result.Data = overview;
            return result;
        }

        /// <summary>
        /// 查詢訂單GET:order/detail/:id
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail/{id}")]
        public ResponseFormat<OrderDetailDto> GetOrderDetail(int id)
        {
            ResponseFormat<OrderDetailDto> result = new ResponseFormat<OrderDetailDto>();
            OrderDetailDto order = new OrderDetailDto();

            try
            {
                string sql = @"
                SELECT A.`Id`
                    ,F.`Name` AS Buyer
                    ,G.`Name` AS Seller
                    ,E.`Name` AS PaymentType
                    ,SUM(C.`Price` * B.`Quantity`) AS Total
                    ,SUM(C.`Cost` * B.`Quantity`) AS Cost
                    ,A.`Address`
                    ,A.`CreateTime`
                    ,D.`Name` AS OrderStatusType 
                FROM `ManagementCenter`.`Order` AS A
                INNER JOIN `ManagementCenter`.`Order_Product_Relation` AS B
                ON A.`Id` = B.`Order_Id`
                INNER JOIN `ManagementCenter`.`Product` AS C
                ON B.`Product_Id` = C.`Id`
                INNER JOIN `ManagementCenter`.`OrderStatusType` AS D
                ON A.`OrderStatusType_Id` = D.`Id`
                INNER JOIN `ManagementCenter`.`PaymentType` AS E
                ON A.`PaymentType_Id` = E.`Id`
                INNER JOIN `ManagementCenter`.`User` AS F
                ON A.`Buyer_Id` = F.`Id`
                INNER JOIN `ManagementCenter`.`User` AS G
                ON A.`Seller_Id` = G.`Id`
                WHERE A.`Id` = @Id
                AND A.`Seller_Id` = @Seller_Id
                GROUP BY A.`Id`";

                MySqlParameter[] parameters = new[]
                {
                    new MySqlParameter("@Id", MySqlDbType.Int32) { Value = id },
                    new MySqlParameter("@Seller_Id", MySqlDbType.Int32) { Value = userId }
                };

                DataTable dtData = sqlFunction.GetData(sql, parameters);

                if (dtData.Rows.Count == 0)
                {
                    result.StatusCode = Error.Code.RESULT_NOT_FOUND;
                    result.Message = Error.Message[Error.Code.RESULT_NOT_FOUND];
                    return result;
                }

                order = sqlFunction.DataTableToList<OrderDetailDto>(dtData)[0];

                order.Products = GetProducts(id);
            }
            catch (Exception e)
            {
                result.StatusCode = Error.Code.DATABASE_ERROR;
                result.Message = e.Message;
                return result;
            }

            result.StatusCode = Error.Code.SUCCESS;
            result.Message = Error.Message[Error.Code.SUCCESS];
            result.Data = order;
            return result;
        }

        /// <summary>
        /// 更新訂單狀態PUT:order/status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("status")]
        public ResponseFormat<string> PutOrdersStatus([FromBody] PutOrderDto request)
        {
            ResponseFormat<string> result = new ResponseFormat<string>();

            try
            {
                // 透過資料驗證模型，確認Request該帶的參數是否都有
                if (!ModelState.IsValid)
                {
                    result.StatusCode = Error.Code.BAD_REQUEST;
                    result.Message = Error.Message[Error.Code.BAD_REQUEST];
                    return result;
                }

                // 確認 Array 內容是否為空
                if (request.Ids.Length == 0)
                {
                    result.StatusCode = Error.Code.BAD_REQUEST;
                    result.Message = Error.Message[Error.Code.BAD_REQUEST];
                    return result;
                }

                foreach (int id in request.Ids)
                {
                    // 確認是否為該筆異動資料的擁有者
                    if (!IsValidOrder(id, userId))
                    {
                        result.StatusCode = Error.Code.BAD_REQUEST;
                        result.Message = Error.Message[Error.Code.BAD_REQUEST];
                        return result;
                    }
                }

                // 將所有 Update 語句組合後，再一起送出執行，減少對 DB 的 Request 次數
                StringBuilder sql = new StringBuilder();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                foreach (int id in request.Ids)
                {
                    sql.Append($@"
                    UPDATE `ManagementCenter`.`Order`
                    SET `OrderStatusType_Id` = 2
                        ,`Admin_Id` = @Admin_Id
                        ,`UpdateTime` = UTC_TIMESTAMP()
                    WHERE `Id` = @Id_{id};");

                    parameters.Add(new MySqlParameter($"@Id_{id}", MySqlDbType.Int32) { Value = id });
                }

                parameters.Add(new MySqlParameter("@Admin_Id", MySqlDbType.Int32) { Value = userId });

                int intAffectRow = sqlFunction.ExecuteSql(sql.ToString(), parameters.ToArray());
            }
            catch (Exception e)
            {
                result.StatusCode = Error.Code.DATABASE_ERROR;
                result.Message = e.Message;
                return result;
            }

            result.StatusCode = Error.Code.SUCCESS;
            result.Message = Error.Message[Error.Code.SUCCESS];
            return result;
        }

        /// <summary>
        /// 更新訂單狀態PUT:order/status/:id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        public ResponseFormat<string> PutOrderStatus(int id)
        {
            ResponseFormat<string> result = new ResponseFormat<string>();

            try
            {
                // 透過資料驗證模型，確認Request該帶的參數是否都有
                if (!ModelState.IsValid)
                {
                    result.StatusCode = Error.Code.BAD_REQUEST;
                    result.Message = Error.Message[Error.Code.BAD_REQUEST];
                    return result;
                }

                // 確認是否為該筆異動資料的擁有者
                if (!IsValidOrder(id, userId))
                {
                    result.StatusCode = Error.Code.BAD_REQUEST;
                    result.Message = Error.Message[Error.Code.BAD_REQUEST];
                    return result;
                }

                string sql = @"
                UPDATE `ManagementCenter`.`Order`
                SET `OrderStatusType_Id` = 2
                    ,`Admin_Id` = @Admin_Id
                    ,`UpdateTime` = UTC_TIMESTAMP()
                WHERE `Id` = @Id";

                MySqlParameter[] parameters = new[]
                {
                    new MySqlParameter("@Id", MySqlDbType.Int32) { Value = id },
                    new MySqlParameter("@Admin_Id", MySqlDbType.Int32) { Value = userId }, // Admin_id欄位，用以紀錄對於該筆資料的最後異動者(情境:在系統存在超級管理員時)
                };

                int intAffectRow = sqlFunction.ExecuteSql(sql, parameters);
            }
            catch (Exception e)
            {
                result.StatusCode = Error.Code.DATABASE_ERROR;
                result.Message = e.Message;
                return result;
            }

            result.StatusCode = Error.Code.SUCCESS;
            result.Message = Error.Message[Error.Code.SUCCESS];
            return result;
        }

        /// <summary>
        /// 取得該筆訂單中所購買的產品資訊
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private List<Product> GetProducts(int orderId)
        {
            List<Product> products = new List<Product>();

            string sql = @"
            SELECT B.`Id`
                ,B.`Name`
                ,B.`Price`
                ,A.`Quantity`
                ,(B.`Price` * A.`Quantity`) AS Total
            FROM `ManagementCenter`.`Order_Product_Relation` AS A
            INNER JOIN `ManagementCenter`.`Product` AS B
            ON A.`Product_Id` = B.`Id`
            WHERE A.`Order_Id` = @Order_Id";

            MySqlParameter[] parameters = new[]
            {
                new MySqlParameter("@Order_Id", MySqlDbType.Int32) { Value = orderId }
            };

            DataTable dtData = sqlFunction.GetData(sql, parameters);

            products = sqlFunction.DataTableToList<Product>(dtData);

            return products;
        }

        /// <summary>
        /// 檢查 Order 是否合理
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool IsValidOrder(int orderId, int userId)
        {
            bool boolFlag = false;

            string sql = @"
            SELECT `Id`
            FROM `ManagementCenter`.`Order`
            WHERE `Id` = @Id
            AND `Seller_Id` = @Seller_Id
            AND `OrderStatusType_Id` = 1";

            MySqlParameter[] parameters = new[]
            {
                new MySqlParameter("@Id", MySqlDbType.Int32) { Value = orderId },
                new MySqlParameter("@Seller_Id", MySqlDbType.Int32) { Value = userId },
            };

            DataTable dtData = sqlFunction.GetData(sql, parameters);

            if (dtData.Rows.Count == 1)
            {
                boolFlag = true;
            }

            return boolFlag;
        }
    }
}
