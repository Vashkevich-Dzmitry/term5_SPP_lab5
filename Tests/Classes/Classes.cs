using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes
{
    public abstract class AbstractClasses
    {
    }

    public class Class : AbstractClasses
    {

    }

    public class ClassWithRecursionDependency : AbstractClasses
    {
        public AbstractClasses RecursiveDependency { get; private set; }

        public ClassWithRecursionDependency(AbstractClasses recursiveDependency)
        {
            RecursiveDependency = recursiveDependency;
        }
    }

    public class AnotherClass
    {

    }
}
