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
            return (TDependency)Resolve(typeof(TDependency));
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
        private object? Resolve(Type tDependency)
        {
            if (RecursionStack.Contains(tDependency))
            {
                throw new StackOverflowException("Recursion");
            }
            RecursionStack.Push(tDependency);

            if (!ImplementationsDictionary.Any())
            {
                return null;
            }

            object? result;
            if (tDependency.IsGenericType &&
                tDependency.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Type argumentType = tDependency.GetGenericArguments()[0];
                var implementations = new List<Implementation>(GetImplementationsForDependency(argumentType));
                var createdArguments = (object[])Activator.CreateInstance(argumentType.MakeArrayType(), new object[] { implementations.Count });
                for (var i = 0; i < implementations.Count; i++)
                {
                    createdArguments[i] = CreateObjectOrGetObjectIfSingleton(implementations[i]);
                }
                result = createdArguments;
            }
            else
            {
                var implementations = new List<Implementation>();
                if (tDependency.IsGenericType &&
                    ImplementationsDictionary.ContainsKey(tDependency.GetGenericTypeDefinition()))
                {
                    implementations = new List<Implementation>(GetImplementationsForDependency(tDependency.GetGenericTypeDefinition()));
                }
                else
                {
                    if (ImplementationsDictionary.ContainsKey(tDependency))
                    {
                        implementations = new List<Implementation>(GetImplementationsForDependency(tDependency));
                    }
                }
                if (implementations.Any())
                {
                    result = CreateObjectOrGetObjectIfSingleton(implementations[0]);
                }
                else
                {
                    result = CreateUsingConstructor(tDependency);
                }
            }
            RecursionStack.Pop();
            return result;
        }

        public List<Implementation>? GetImplementationsForDependency(Type tDependency)
        {
            if (ImplementationsDictionary.ContainsKey(tDependency))
            {
                return ImplementationsDictionary[tDependency];
            }
            return null;
        }

        private object CreateObjectOrGetObjectIfSingleton(Implementation implementation)
        {
            if (implementation.IsSingleton)
            {
                if (implementation.SingletonObject == null)
                {
                    lock (implementation)
                    {
                        if (implementation.SingletonObject == null)
                        {
                            implementation.SingletonObject = Resolve(implementation.Type);
                            return implementation.SingletonObject;
                        }
                    }
                }
                return implementation.SingletonObject;
            }
            else
            {
                return Resolve(implementation.Type);
            }
        }
        private object CreateUsingConstructor(Type type)
        {
            object? result = null;

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                var genericParams = genericArguments.Select(dependency =>
                {
                    var implementations = DConfig
                        .GetImplementationsForDependency(dependency.BaseType!)?.ToArray();
                    if (implementations == null)
                    {
                        return dependency.BaseType;
                    }
                    else
                    {
                        return implementations.First().Type;
                    }
                }).ToArray();

                type = type.MakeGenericType(genericParams);
            }

            var constructorsInfo = type.GetConstructors();
            foreach (var constructorInfo in constructorsInfo)
            {
                var parameters = new List<object>();

                try
                {
                    var paramsInfo = constructorInfo.GetParameters();
                    foreach (var paramInfo in paramsInfo)
                    {
                        parameters.Add(Resolve(paramInfo.ParameterType));
                    }

                    result = Activator.CreateInstance(type, parameters.ToArray());
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (StackOverflowException e)
                {
                    throw e;
                }
                catch { }
            }
            return result;
        }
    }
}
