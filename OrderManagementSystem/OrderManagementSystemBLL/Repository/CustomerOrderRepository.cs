using Dapper;
using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemBLL.Repository
{
    public class CustomerOrderRepository
    {
        private string _connectionString;
        public CustomerOrderRepository()
        {
            _connectionString = "Server=PCA161\\SQL2019;database=New_Project;User Id=sa;password=Tatva@123;";
        }
        //public List<customer> GetAllCustomerOrders()
        //{
        //    string query = "SELECT order_id, product_name, customer_id, quantity, order_amount FROM [order] WHERE isDeleted = 0";

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        // Execute the query using Dapper
        //        var orders = connection.Query<customer>(query).ToList();

        //        return orders;
        //    }
        //}
        public List<CustomerOrder> GetAllCustomerOrders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Execute the stored procedure using Dapper
                var orders = connection.Query<CustomerOrder>("GetCustomerOrders", commandType: CommandType.StoredProcedure).ToList();

                return orders;
            }
        }

    }
}
