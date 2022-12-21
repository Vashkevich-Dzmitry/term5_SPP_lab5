namespace DependencyInjector
{
    public class Implementation
    {
        public Type Type { get; set; }
        public bool IsSingleton { get; set; }
        public object? SingletonObject { get; set; }
        public Implementation(Type implementation, bool isSingleton)
        {
            Type = implementation;
            IsSingleton = isSingleton;
            SingletonObject = null;
        }
    }
}