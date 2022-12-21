namespace DependencyInjector
{
    public interface IDependencyContainer
    {
        public Dictionary<Type, List<Implementation>> ImplementationsDictionary { get; set; }
        public Stack<Type> RecursionStack { get; set; }

        public TDependency Resolve<TDependency>();
        public void Register<TDependency, TImplementation>(bool isSingleton);
    }
}
