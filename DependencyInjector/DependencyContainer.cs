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

        public void Register<TDependency, TImplementation>(bool isSingleton = false)
        {
            Register(typeof(TDependency), typeof(TImplementation), isSingleton);
        }

        public void Register(Type tDependency, Type tImplementation, bool isSingleton = false)
        {
            if (tImplementation.IsAbstract)
            {
                throw new ArgumentException("Implementation is abstract");
            }

            if (!tDependency.IsAssignableFrom(tImplementation)
                && !tDependency.IsGenericTypeDefinition
                && !tImplementation.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Dependency is not assignable from implementation");
            }

            if (!ImplementationsDictionary.TryGetValue(tDependency, out List<Implementation> dependencyImplementations))
            {
                dependencyImplementations = new List<Implementation>();
                ImplementationsDictionary[tDependency] = dependencyImplementations;
            }
            dependencyImplementations.Add(new Implementation(tImplementation, isSingleton));
        }
    }
}
