using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjector
{
    public class DependencyContainer : IDependencyContainer
    {
        public Dictionary<Type, List<Implementation>> ImplementationsDictionary { get; set; }
        public Stack<Type> RecursionStack { get; set; }

        public DependencyContainer()
        {
            ImplementationsDictionary = new Dictionary<Type, List<Implementation>>();
            RecursionStack = new Stack<Type>();
        }

        public TDependency Resolve<TDependency>()
        {
            throw new NotImplementedException();
        }

        public void Register<TDependency, TImplementation>(bool isSingleton)
        {
            throw new NotImplementedException();
        }
    }
}
