using Dapper;
using Microsoft.AspNetCore.Http;
using OrderManagementSystemDAL.Models;
using OrderManagementSystemDAL.Models.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemBLL.Repository
{
    public class CustomerRepository
    {
        private SqlConnection _connectionString;

        public CustomerRepository()
        {
            string connectionStr = "Server=PCA161\\SQL2019;database=New_Project;User Id=sa;password=Tatva@123;";
            _connectionString = new SqlConnection(connectionStr);

        }
        
        public string UploadFile(IFormFile ProfilePhoto)
        {
            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                // Check if the file type is allowed
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png",".jfif" };
                string fileExtension = Path.GetExtension(ProfilePhoto.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return "Only JPG, JPEG, and PNG files are allowed.";
                }

                // Generate a unique file name
                string fileName = Guid.NewGuid().ToString() + fileExtension;

                // Save the file to the uploads directory
                string uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                string filePath = Path.Combine(uploadsDirectory, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePhoto.CopyTo(stream);
                }

                return "/uploads/" + fileName;
            }
            return null;
        }


        public List<customer> GetAllCustomers()
        {
            List<customer> customerList = new List<customer>();

            string query = "SELECT customer_id, profileImgPath, customer_name, customer_email, customer_location FROM customer WHERE isDeleted = 0";

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
                            profileImgPath = reader["profileImgPath"].ToString(),
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
        public bool AddCustomer(CustomerDTO customer)
        {
            string query = "INSERT INTO customer (profileImgPath,customer_name, customer_email, customer_location,isDeleted) VALUES (@profileImgPath,@customer_name, @customer_email, @customer_location,0)";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@profileImgPath", customer.profileImgPath);
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

            string query = "SELECT customer_id, profileImgPath, customer_name, customer_email, customer_location FROM customer where customer_id=@customer_id";

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
                            profileImgPath = reader["profileImgPath"].ToString(),
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

        public bool EditCustomerDetails(CustomerByIdDTO customer)
        {
            string query = "UPDATE customer set profileImgPath =@profileImgPath, customer_name=@customer_name, customer_email=@customer_email, customer_location=@customer_location where customer_id=@customer_id";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@customer_id", customer.customer_id);
                cmd.Parameters.AddWithValue("@profileImgPath", customer.profileImgPath);
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
            string query = "UPDATE customer SET isDeleted = 1 WHERE customer_id = @customer_id";

            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
            {
                cmd.Parameters.AddWithValue("@customer_id", Id);

                _connectionString.Open();

                int rowsAffected = cmd.ExecuteNonQuery();

                _connectionString.Close();

                return rowsAffected > 0;
            }
        }
        public bool DeleteSelectedCustomerDetails(string customerids)
        {
            try
            {
                using (var connection = _connectionString)
                {
                    connection.Open();
                    string[] cid = customerids.Split(',');


                    for (int i = 0; i < cid.Count(); ++i)
                    {
                        using (var command = new SqlCommand("DeleteSelectedCustomer", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Id", int.Parse(cid[i]));

                            int rowsAffected = command.ExecuteNonQuery();
                        }

                    }
                    return true;


                }
            }
            catch (Exception e)
            {
                return false;
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
    //public Stream GetCustomerImage(int customer_id)
    //{
    //    string query = "SELECT profileImgPath FROM customer WHERE customer_id = @CustomerId";
    //    using (SqlCommand cmd = new SqlCommand(query, _connectionString))
    //    {
    //        _connectionString.Open();
    //        using (var command = new SqlCommand(query, _connectionString))
    //        {
    //            command.Parameters.AddWithValue("@customer_id", customer_id);
    //            using (var reader = command.ExecuteReader())
    //            {
    //                if (reader.Read())
    //                {
    //                    var imageData = (SqlBinary)reader["profileImgPath"];
    //                    return imageData.Value;
    //                }
    //            }
    //        }
    //    }
    //    return null; // Return null if no image found or error occurred
    //}

    //public string UploadImage(IFormFile file)
    //{
    //    if (file != null && file.Length > 0)
    //    {
    //        string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CustomerProfilePhotos");
    //        if (!Directory.Exists(uploadsPath))
    //        {
    //            Directory.CreateDirectory(uploadsPath);
    //        }

    //        string fileName = FileUploadServices.GetFilenameWithoutExtension(file.FileName) + Guid.NewGuid() + Path.GetExtension(file.FileName);
    //        string filePath = Path.Combine(uploadsPath, fileName);

    //        using (var fileStream = new FileStream(filePath, FileMode.Create))
    //        {
    //            file.CopyTo(fileStream);
    //        }

    //        return "/CustomerProfilePhotos/" + fileName;
    //    }
    //    return null;
    //}
    //public bool uploadImage()
    //{
    //    customer customerDetails = new customer();
    //    if (customerDetails.profileImgPath != null)
    //    {
    //        try
    //        {
    //            string extractedFileName = Path.GetFileName(customerDetails.profileImgPath);
    //            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", extractedFileName);

    //            string query = "UPDATE customer SET profileImgPath = @profileImgPath WHERE customer_id = @CustomerId";
    //            using (SqlCommand cmd = new SqlCommand(query, _connectionString))
    //            {
    //                _connectionString.Open();

    //                // Create a parameterized query to insert the file path into the database

    //                using (var command = new SqlCommand(query, _connectionString))
    //                {
    //                    command.Parameters.AddWithValue("@profileImgPath", filePath);
    //                    command.Parameters.AddWithValue("@CustomerId", customerDetails.customer_id); // Assuming you have the customer ID available

    //                    int rowsAffected = command.ExecuteNonQuery();

    //                    if (rowsAffected > 0)
    //                    {
    //                        // Save the file to the file system
    //                        //using (var stream = System.IO.File.Create(filePath))
    //                        //using (FileStream stream = new FileStream("filePath", FileMode.Create))
    //                        //using (FileStream stream = System.IO.File.Create(filePath))
    //                        //{
    //                        //    customerDetails.profileImgPath.CopyTo(stream);
    //                        //}
    //                        //return true;

    //                        string sourceFilePath = customerDetails.profileImgPath; // Assuming profileImgPath is a string representing the file path
    //                        string destinationFilePath = "wwwroot\\uploads"; // Provide the destination path where you want to save the file

    //                        // Create a FileStream to read the source file
    //                        using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
    //                        {
    //                            // Create a FileStream to write to the destination file
    //                            using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
    //                            {
    //                                // Copy the data from the source stream to the destination stream
    //                                sourceStream.CopyTo(destinationStream);
    //                            }
    //                        }

    //                    }
    //                    else
    //                    {
    //                        return false; // Customer not found or no rows updated
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle exceptions here
    //            Console.WriteLine("Error: " + ex.Message);
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        return false; // No file uploaded
    //    }
    //}

    //public string UploadFile(IFormFile? ProfilePhoto)
    //{
    //    if (ProfilePhoto != null && ProfilePhoto.Length > 0)
    //    {
    //        var extractedFileName = Path.GetFileName(ProfilePhoto.FileName);
    //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads", extractedFileName);
    //        using (var stream = System.IO.File.Create(filePath))
    //        {
    //            ProfilePhoto.CopyTo(stream);
    //        }


    //        return "/uploads/" + extractedFileName;
    //    }
    //    return null;
    //}

}
