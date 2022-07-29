namespace ServiceA
{
    class Program
    {
        private const string QUEUE_NAME = "wongaQueue";
        static void Main(string[] args)
        {
            PublisherClient queuePublisher = new(QUEUE_NAME);
            string name;
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
