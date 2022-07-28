using System.Text;

namespace Utils
{
    public class RabbitMQUtil
    {
        public static string HostName { get; set; }
        public static string Message { get; set; }
        public static string QueueName { get; set; }
        private byte[] Body()
        {
            return Encoding.UTF8.GetBytes(Message);

        }
        public List<string> GetValidationList(string list)
        {
            List<string> result = new List<string>();

            string[] terms = list.Split(new char[] { '|' });
            for (int i = 0; i < terms.Length; ++i)
            {
                string term = terms[i];
                result.Add(term);
            }

            return result;
        }

    }
}