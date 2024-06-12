using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace OrderManagementSystemBLL.Repository
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
            //string query = "SELECT order_id, product_name, customer_id, quantity, order_amount FROM [order] WHERE isDeleted = 0";
            string query = "SELECT o.order_id, o.product_name, c.customer_name, o.quantity, o.order_amount,o.total_amount FROM[order] o JOIN customer c ON o.customer_id = c.customer_id WHERE o.isDeleted = 0";


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Execute the query using Dapper
                var orders = connection.Query<order>(query).ToList();

                return orders;
            }
        }

        //public List<order> GetAllOrders()
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        // Call the stored procedure using Dapper
        //        var orders = connection.Query<order>("ShowOrderAndCustomerOrder", commandType: CommandType.StoredProcedure).ToList();

        //        return orders;
        //    }
        //}


        //public bool AddOrder(order order)
        //{
        //    string query = @"
        //            INSERT INTO [Order] (product_name, customer_id, quantity, order_amount, isDeleted,total_amount)VALUES (@product_name,@customer_id, @quantity, @order_amount,0,@total_amount)";

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        int rowsAffected = connection.Execute(query, order);


        //        decimal totalAmount = order.quantity * order.order_amount;

        //        // insert the total_amount separately
        //        string updateTotalAmountQuery = "INSERT INTO [Order] (total_amount) VALUES (@total_amount);";
        //        rowsAffected += connection.Execute(updateTotalAmountQuery, new { total_amount = totalAmount, order_id = order.order_id });

        //        return rowsAffected > 0;
        //    }
        //}
        public bool AddOrder(order order)
        {
            // Calculate the total amount
            decimal totalAmount = order.quantity * order.order_amount;

            // Construct the query to insert the order
            string query = @"
        INSERT INTO [Order] (product_name, customer_id, quantity, order_amount, isDeleted, total_amount)
        VALUES (@product_name, @customer_id, @quantity, @order_amount, 0, @total_amount)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Execute the insert query
                int rowsAffected = connection.Execute(query, new
                {
                    order.product_name,
                    order.customer_id,
                    order.quantity,
                    order.order_amount,
                    total_amount = totalAmount
                });

                // Return true if rows were affected (i.e., the insert was successful)
                return rowsAffected > 0;
            }
        }


        //public bool AddOrder(order order)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        // Call the stored procedure
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@product_name", order.product_name);
        //        parameters.Add("@customer_id", order.customer_id);
        //        parameters.Add("@quantity", order.quantity);
        //        parameters.Add("@order_amount", order.order_amount);
        //        parameters.Add("@TotalAmount ", ((order.quantity)* (order.order_amount)));

        //        int rowsAffected = connection.Execute("InsertOrderAndCustomerOrder", parameters, commandType: CommandType.StoredProcedure);

        //        return rowsAffected > 0;
        //    }
        //}



        public order GetOrderById(int Id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT order_id,product_name, customer_id, quantity, order_amount,total_amount FROM [order] WHERE order_id = @order_id AND isDeleted = 0";

                return connection.QueryFirstOrDefault<order>(query, new { order_id = Id });
            }
        }


        //public bool EditOrderDetails(int Id, order order)
        //{
        //    string query = "UPDATE [order] SET product_name=@product_name, customer_id=@customer_id, quantity=@quantity, order_amount=@order_amount, total_amount=@total_amount WHERE order_id=@order_id";

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        int rowsAffected = connection.Execute(query, new { product_name = order.product_name, customer_id = order.customer_id, quantity = order.quantity, order_amount = order.order_amount, total_amount = (order.quantity * order.order_amount), order_id = Id });

        //        return rowsAffected > 0;
        //    }
        //}
        public bool EditOrderDetails(int Id, order order)
        {
            // Update the other columns except total_amount
            string query = "UPDATE [order] SET product_name=@product_name, customer_id=@customer_id, quantity=@quantity, order_amount=@order_amount WHERE order_id=@order_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int rowsAffected = connection.Execute(query, new { product_name = order.product_name, customer_id = order.customer_id, quantity = order.quantity, order_amount = order.order_amount, order_id = Id });

                // Recalculate total_amount based on the updated values of quantity and order_amount
                decimal totalAmount = order.quantity * order.order_amount;

                // Update the total_amount separately
                string updateTotalAmountQuery = "UPDATE [order] SET total_amount=@total_amount WHERE order_id=@order_id";
                rowsAffected += connection.Execute(updateTotalAmountQuery, new { total_amount = totalAmount, order_id = Id });

                return rowsAffected > 0;
            }
        }


        //public bool EditOrderDetails(int Id, order order)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var parameters = new
        //        {
        //            order_id = Id,
        //            product_name = order.product_name,
        //            customer_id = order.customer_id,
        //            quantity = order.quantity,
        //            order_amount = order.order_amount,
        //            //customer_order_id = order.customer_order_id,
        //            TotalAmount = ((order.quantity) * (order.order_amount))
        //    };

        //        int rowsAffected = connection.Execute("UpdateOrderAndCustomerOrder", parameters, commandType: CommandType.StoredProcedure);

        //        return rowsAffected > 0;
        //    }
        //}


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
        public bool DeleteSelectedOrderDetails(string orderids)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string[] cid = orderids.Split(',');

                    foreach (var id in cid)
                    {
                        connection.Execute("DeleteSelectedOrder", new { Id = int.Parse(id) }, commandType: CommandType.StoredProcedure);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                // Log or handle the exception as needed
                return false;
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
