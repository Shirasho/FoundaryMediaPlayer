using System;
using FluentAssertions;
using Foundary.CommandParser;
using Ninject;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// An application dependency resolver that utilizes the DI container.
    /// </summary>
    public sealed class ApplicationDependencyResolver : IDependencyResolver
    {
        private IKernel _Kernel {get;}

        public ApplicationDependencyResolver(IKernel kernel)
        {
            kernel.Should().NotBeNull();

            _Kernel = kernel;
        }

        /// <inheritdoc />
        public IConsoleCommand Resolve(Type type)
        {
            return _Kernel.Get(type) as IConsoleCommand;
        }
    }
}
