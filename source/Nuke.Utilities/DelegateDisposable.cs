// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Nuke.Common.Utilities
{
    /// <summary>
    /// Represents an <see cref="IDisposable"/> that executes a delegate upon <see cref="Dispose"/>.
    /// </summary>
    public class DelegateDisposable : IDisposable
    {
        /// <summary>
        /// Creates an <see cref="IDisposable"/> from a setup and cleanup delegate.
        /// </summary>
        public static IDisposable CreateBracket([InstantHandle] Action setup = null, [InstantHandle] Action cleanup = null)
        {
            setup?.Invoke();
            return new DelegateDisposable(cleanup);
        }

        [CanBeNull] private readonly Action _cleanup;

        private DelegateDisposable([CanBeNull] Action cleanup)
        {
            _cleanup = cleanup;
        }

        public void Dispose()
        {
            _cleanup?.Invoke();
        }
    }
}
