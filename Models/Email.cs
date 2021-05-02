namespace WebAPIService.Models
{
    public class Email                                  //Модель нашей таблицы полностью повторяющая столбцы в таблице БД
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Carbon_copy_recipients { get; set; }
        public string Status { get; set; }
    }
}
