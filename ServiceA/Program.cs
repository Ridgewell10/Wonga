namespace ServiceA
{
    class Program
    {
        private const string QUEUE_NAME = "myQueue";
        static void Main(string[] args)
        {
            string name = string.Empty;
            Publisher queuePublisher = new Publisher(QUEUE_NAME);
            do
            {
                Console.Write("Enter a name ['q' to quit]: ");
                name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name) && name.ToLower() != "q")
                {
                    queuePublisher.PublishMessage($"Hello my name is, {name}");
                    Console.Clear();
                    Console.WriteLine("Published Message: {0}", name);
                }
            } while (name.ToLower() != "q");
        }
    }

}
