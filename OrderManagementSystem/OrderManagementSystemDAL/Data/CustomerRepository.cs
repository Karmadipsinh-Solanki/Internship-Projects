using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Data
{
    public class CustomerRepository
    {
        private SqlConnection _connectionString;

        public CustomerRepository()
        {
            string connectionStr = "Server=PCA161\\SQL2019;database=New_Project;User Id=sa;password=Tatva@123;";
            _connectionString = new SqlConnection(connectionStr);

        }


        //public List<customer> GetAllCustomers()
        //{
        //    List<customer> customerList = new List<customer>();

        //    SqlCommand cmd = new SqlCommand("AddCustomer", _connectionString);
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

        //    DataTable dt = new DataTable();

        //    dataAdapter.Fill(dt);

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        customerList.Add(
        //            new customer
        //            {
        //                customer_id = Convert.ToInt32(dr["customer_id"]),
        //                customer_name = dr["customer_name"].ToString(),
        //                customer_location = dr["customer_location"].ToString(),
        //                customer_email = dr["customer_email"].ToString(),


        //            });
        //    }
        //    return customerList;
        //}
        public List<customer> GetAllCustomers()
        {
            List<customer> customerList = new List<customer>();

            string query = "SELECT customer_id, customer_name, customer_email, customer_location FROM customer WHERE isDeleted = 0";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                _connectionString.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerList.Add(new customer
                        {
                            customer_id = Convert.ToInt32(reader["customer_id"]),
                            customer_name = reader["customer_name"].ToString(),
                            customer_email = reader["customer_email"].ToString(),
                            customer_location = reader["customer_location"].ToString()
                        });
                    }
                }

                _connectionString.Close();
            }

            return customerList;
        }


        //public bool AddCustomer(customer customer)
        //{
        //    SqlCommand cmd = new SqlCommand("AddCustomer", _connectionString);
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@customer_id", customer.customer_id);
        //    cmd.Parameters.AddWithValue("@customer_name", customer.customer_name);
        //    cmd.Parameters.AddWithValue("@customer_email", customer.customer_email);
        //    cmd.Parameters.AddWithValue("@customer_location", customer.customer_location);

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
        public bool AddCustomer(customer customer)
        {
            string query = "INSERT INTO customer (customer_id,customer_name, customer_email, customer_location,isDeleted) VALUES (@customer_id,@customer_name, @customer_email, @customer_location,0)";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@customer_id", customer.customer_id);
                cmd.Parameters.AddWithValue("@customer_name", customer.customer_name);
                cmd.Parameters.AddWithValue("@customer_email", customer.customer_email);
                cmd.Parameters.AddWithValue("@customer_location", customer.customer_location);

                _connectionString.Open();

                int i = cmd.ExecuteNonQuery();

                _connectionString.Close();

                return i >= 1;
            }
        }

        public customer GetCustomerById(int Id)
        {
            customer customerList = new customer();

            string query = "SELECT customer_id, customer_name, customer_email, customer_location FROM customer where customer_id=@customer_id";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                _connectionString.Open();

                SqlParameter param;

                cmd.Parameters.Add(new SqlParameter("@customer_id", Id));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerList = new customer
                        {
                            customer_id = Convert.ToInt32(reader["customer_id"]),
                            customer_name = reader["customer_name"].ToString(),
                            customer_email = reader["customer_email"].ToString(),
                            customer_location = reader["customer_location"].ToString()
                        };
                    }
                }


                _connectionString.Close();
            }

            return customerList;
        }
        
        public bool EditCustomerDetails(int Id, customer customer)
        {
            string query = "UPDATE customer set customer_id=@customer_id, customer_name=@customer_name, customer_email=@customer_email, customer_location=@customer_location where customer_id=@customer_id";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@customer_id", customer.customer_id);
                cmd.Parameters.AddWithValue("@customer_name", customer.customer_name);
                cmd.Parameters.AddWithValue("@customer_email", customer.customer_email);
                cmd.Parameters.AddWithValue("@customer_location", customer.customer_location);

                _connectionString.Open();

                int i = cmd.ExecuteNonQuery();

                _connectionString.Close();

                return i >= 1;
            }
        }
        public bool DeleteCustomerDetails(int Id)
        {
            string query = "DELETE FROM customer WHERE customer_id = @customer_id";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@customer_id", Id);

                _connectionString.Open();

                int rowsAffected = cmd.ExecuteNonQuery();

                _connectionString.Close();

                return rowsAffected > 0;
            }
        }




        //public List<Customer> GetAllCustomers()
        //{
        //    List<Customer> customers = new List<Customer>();

        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT Id, Name, Email FROM Customers";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            connection.Open();

        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    customers.Add(new Customer
        //                    {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        Name = reader["Name"].ToString(),
        //                        Email = reader["Email"].ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return customers;
        //}
    }
}
