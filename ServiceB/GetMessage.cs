using System.Text.RegularExpressions;

namespace ServiceB
{
    public class GetMessage
    {

        /// <summary>
        /// Extracts name from message if name is valid
        /// </summary>
        /// <param name="message">The message received as str
        public static string GetName(string message)
        {
            if (!IsValidMessage(message))
                throw new ArgumentException(message);
            else
                return message.Split(',')[1].Trim();
        }

        /// <summary>
        /// Validates Message format
        /// </summary>
        /// <param name="message">The message to be validated as string</param>
        /// <returns>Indicates if message is in correct format</returns>
        public static bool IsValidMessage(string message) => Regex.IsMatch(message, @"^Hello my name is,\s[a-zA-Z ]*$");
    }
}
