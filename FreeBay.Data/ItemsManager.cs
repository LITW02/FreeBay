using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBay.Data
{
    public class ItemsManager
    {
        private string _connectionString;

        public ItemsManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Item> GetItems()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Items";
                connection.Open();
                List<Item> items = new List<Item>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Item item = new Item();
                    item.Id = (int)reader["Id"];
                    item.Name = (string)reader["Name"];
                    item.Description = (string)reader["Description"];
                    item.PhoneNumber = (string)reader["PhoneNumber"];
                    item.DateCreated = (DateTime)reader["DateCreated"];
                    items.Add(item);
                }

                return items.OrderByDescending(i => i.DateCreated);
            }
        }

        public int Add(Item item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Items (Name, PhoneNumber, Description, DateCreated) " +
                                      "VALUES(@name, @phone, @desc, @date); SELECT @@Identity";
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@phone", item.PhoneNumber);
                command.Parameters.AddWithValue("@desc", item.Description);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }

        public void Delete(int itemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Items WHERE Id = @id";
                command.Parameters.AddWithValue("@id", itemId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
