using NUnit.Framework;
using DependencyInjector;
using Tests.Classes;
using System;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void NotSingltonRealizations()
        {
            DependencyContainer container = new();
            container.Register<AbstractClasses, Class>(false);
            var obj1 = container.Resolve<AbstractClasses>();
            var obj2 = container.Resolve<AbstractClasses>();

            Assert.True(!obj1.Equals(obj2));
        }

        [Test]
        public void SingltonRealizations()
        {
            DependencyContainer container = new();
            container.Register<AbstractClasses, Class>(true);
            var obj1 = container.Resolve<AbstractClasses>();
            var obj2 = container.Resolve<AbstractClasses>();

            Assert.True(obj1.Equals(obj2));
        }

        [Test]
        public void ClassWithRecursion()
        {
            DependencyContainer container = new();
            container.Register<AbstractClasses, ClassWithRecursionDependency>(true);

            Assert.Throws<StackOverflowException>(() => container.Resolve<AbstractClasses>());
        }

        [Test]
        public void EmptyContainer()
        {
            DependencyContainer container = new();
            Assert.Null(container.Resolve<Class>());
        }
    }
}