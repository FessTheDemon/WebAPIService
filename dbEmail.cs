using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebAPIService.Models;

namespace WebAPIService
{


    public interface IEmailRepository                   //Создаем интерфейс для методов обработки данных для БД
    {
        void Create(Email email);
        List<Email> GetEmails();
    }
    public class EmailRepository : IEmailRepository
    {
        string connectionString = null;
        public EmailRepository(string conn)
        {
            connectionString = conn;
        }
        public List<Email> GetEmails()                                          //Создаем метод для вывода всех строк и столбцов из таблицы с имейлами с помощью Дэппера
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Email>("SELECT * FROM emails").ToList();
            }
        }

        public void Create(Email email)                                         //Создаем метод для занесения данных об имейле в нашу таблицу
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "INSERT INTO emails (Recipient, Subject, Text, Carbon_copy_recipients, Status) VALUES(@Recipient, @Subject, @Text, @Carbon_copy_recipients, @Status)";
                db.Execute(sqlQuery, email);
            }
        }
    }
}
