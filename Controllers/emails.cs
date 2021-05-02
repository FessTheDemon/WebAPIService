using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Mail;
using WebAPIService.Models;

namespace WebAPIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class emails : ControllerBase
    {
        private static JToken GetJsonKey(JObject json, string str) => JObject.Parse(json.ToString())[str];         //Создаем пользовательский метод для поиска значения по ключу внутри json файла

        IEmailRepository repo;
        public emails(IEmailRepository rep)
        {
            repo = rep;
        }

        [HttpGet]
        public IActionResult Get()                      //Реализация GET-запроса с вызовом метода на вывод нашей таблицы БД
        {
            return Ok(repo.GetEmails());
        }


        [HttpPost]
        public IActionResult Post(JObject json)         //Реализация POST-запроса с отправкой мейла и занесением его в БД
        {

            var recipient = GetJsonKey(json, "recipient");                              //Записываем найденные по ключам значения в отдельные переменные для будущего письма
            var subject = GetJsonKey(json, "subject");
            var text = GetJsonKey(json, "text");
            var carbon_copy_recipients = GetJsonKey(json, "carbon_copy_recipients");
            string status;

            if (subject == null || text == null || recipient == null || subject.ToString() == "" || text.ToString() == "" || recipient.ToString() == "")    //Проверка "на дурака"
                status = "Не заполнены обязательные поля или заполнены неверно!";
            else
            {

                MailAddress from = new MailAddress("email@example.com");        //Вносим имеил отправителя и получателя, а после реализуем экземпляр класса MailMessage
                MailAddress to = new MailAddress(recipient.ToString());
                MailMessage m = new MailMessage(from, to);

                if (carbon_copy_recipients != null)                         //Вносим в копии полученные из файла мейлы, если они есть
                    foreach (var recip in carbon_copy_recipients)
                        m.CC.Add(recip.ToString());

                m.Subject = subject.ToString();                     //Вносим в тему и тело полученные тему и тело
                m.Body = text.ToString();
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);        //Подключаемся к серверу, через который будем отправлять мейлы

                var credential = new NetworkCredential          //Вводим почту и пароль, с которой будем отправлять мейлы
                {
                    UserName = "email@example.com",
                    Password = "password/admin"
                };
                smtp.Credentials = credential;
                smtp.EnableSsl = true;

                try             //Если не будет исключений, то отправляем наше письмо и ставим статус ОК, если есть исключение, то сообщение не отправляется, а в статус ставится полученная ошибка
                {
                    smtp.Send(m);
                    status = "Ok";
                }
                catch (Exception o)
                {
                    status = "Ошибка!\n" + o.Message;
                }

            }




            Email email = new Email();          //Создаем экземпляр класса Email


            if (recipient == null || recipient.ToString() == "")    //Заполняем в нашем мейле получателя, тему, текст и копии, а если они пустые то вносим в переменную сообщение об ошибке
                email.Recipient = "Отсутствует получатель";
            else
                email.Recipient = recipient.ToString();


            if (subject == null || subject.ToString() == "")
                email.Subject = "Отсутствует заголовок";
            else
                email.Subject = subject.ToString();


            if (text == null || text.ToString() == "")
                email.Text = "Отсутствует тело письма";
            else
                email.Text = text.ToString();

            if (carbon_copy_recipients != null)
                email.Carbon_copy_recipients = String.Join(", ", carbon_copy_recipients);

            email.Status = status;

            repo.Create(email);     //Создаем строку с отправленным или нет письмом в нашей БД


            return Ok("Дело сделано!");
        }

    }
}
