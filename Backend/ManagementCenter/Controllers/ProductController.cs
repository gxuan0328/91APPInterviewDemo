using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ManagementCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace ManagementCenter.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private SqlFunction sqlFunction;
        private int userId;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            sqlFunction = new SqlFunction(_configuration);
            //因目前未實作身分識別機制，暫以此模擬從Middleware解讀出來的使用者資訊
            userId = 1;
        }

        /// <summary>
        /// 查詢產品GET:product/detail/:id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail/{id}")]
        public ResponseFormat<ProductDetailDto> GetProductDetail(int id)
        {
            ResponseFormat<ProductDetailDto> result = new ResponseFormat<ProductDetailDto>();
            ProductDetailDto product = new ProductDetailDto();

            try
            {
                string sql = @"
                SELECT `Id`
                    ,`Name`
                    ,`Description`
                    ,`Cost`
                    ,`Price`
                    ,`CreateTime`
                FROM `ManagementCenter`.`Product`
                WHERE `Id` = @Id";

                MySqlParameter[] parameters = new[]
                {
                    new MySqlParameter("@Id", MySqlDbType.Int32) { Value = id }
                };

                DataTable dtData = sqlFunction.GetData(sql, parameters);

                if (dtData.Rows.Count == 0)
                {
                    result.StatusCode = Error.Code.RESULT_NOT_FOUND;
                    result.Message = Error.Message[Error.Code.RESULT_NOT_FOUND];
                    return result;
                }

                product = sqlFunction.DataTableToList<ProductDetailDto>(dtData)[0];
            }
            catch (Exception e)
            {
                result.StatusCode = Error.Code.DATABASE_ERROR;
                result.Message = e.Message;
                return result;
            }

            result.StatusCode = Error.Code.SUCCESS;
            result.Message = Error.Message[Error.Code.SUCCESS];
            result.Data = product;
            return result;
        }
    }
}
