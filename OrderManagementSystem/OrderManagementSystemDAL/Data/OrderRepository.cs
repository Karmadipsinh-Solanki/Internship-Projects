using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace OrderManagementSystemDAL.Data
{
    public class OrderRepository
    {
        private string _connectionString;

        public OrderRepository()
        {
            _connectionString = "Server=PCA161\\SQL2019;database=New_Project;User Id=sa;password=Tatva@123;";

        }


        public List<order> GetAllOrders()
        {
            string query = "SELECT order_id, product_name, customer_id, quantity, order_amount FROM [order] WHERE isDeleted = 0";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Execute the query using Dapper
                var orders = connection.Query<order>(query).ToList();

                return orders;
            }
        }


        //public bool Addorder(order order)
        //{
        //    SqlCommand cmd = new SqlCommand("Addorder", _connectionString);
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@order_id", order.order_id);
        //    cmd.Parameters.AddWithValue("@order_name", order.order_name);
        //    cmd.Parameters.AddWithValue("@order_email", order.order_email);
        //    cmd.Parameters.AddWithValue("@order_location", order.order_location);

        //    _connectionString.Open();

        //    int i = cmd.ExecuteNonQuery();
        //    _connectionString.Close();

        //    if (i>=1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public bool Addorder(order order)
        {
            string query = "INSERT INTO order (order_id,product_name, customer_id, quantity,order_amount,isDeleted) VALUES (@order_id,@product_name, @customer_id, @quantity,@order_amount,0)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int rowsAffected = connection.Execute(query, order);

                return rowsAffected > 0;
            }
        }

        public order GetOrderById(int Id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT order_id, product_name, customer_id, quantity, order_amount FROM [order] WHERE order_id = @order_id AND isDeleted = 0";

                return connection.QueryFirstOrDefault<order>(query, new { order_id = Id });
            }
        }

        public bool EditOrderDetails(int Id, order order)
        {
            string query = "UPDATE order set order_id=@order_id, product_name=@product_name, customer_id=@customer_id, quantity=@quantity, order_amount=@order_amount where order_id=@order_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int rowsAffected = connection.Execute(query, new { order_id = Id, product_name = order.product_name, customer_id = order.customer_id, quantity = order.quantity, order_amount=order.order_amount });

                return rowsAffected > 0;
            }
        }

        public bool DeleteOrderDetails(int Id)
        {
            string query = "UPDATE [order] SET isDeleted = 1 WHERE order_id = @order_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int rowsAffected = connection.Execute(query, new { order_id = Id });

                return rowsAffected > 0;
            }
        }




        //public List<order> GetAllorders()
        //{
        //    List<order> orders = new List<order>();

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT Id, Name, Email FROM orders";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            connection.Open();

        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    orders.Add(new order
        //                    {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        Name = reader["Name"].ToString(),
        //                        Email = reader["Email"].ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return orders;
        //}
    }
}
