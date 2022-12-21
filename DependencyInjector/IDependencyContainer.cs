using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
