namespace SamplePeer
{
    public class Header
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public Header(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
