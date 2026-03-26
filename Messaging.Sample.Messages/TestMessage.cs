namespace Messaging.Sample.Messages
{
    public class TestMessage
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Role { get; set; }

        public override string ToString()
        {
            return $"{Name} {Id} {Role}";
        }
    }

    public class TestMessage2
    {
        public int Id { get; set; }
    }
}