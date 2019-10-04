﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

// ReSharper disable once CheckNamespace
namespace Xunit.Sdk
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Abstractions;

    /// <summary>
    /// The discovery class for <see cref="WpfFactAttribute"/>
    /// </summary>
    public class WpfFactDiscoverer : FactDiscoverer
    {
        private readonly IMessageSink diagnosticMessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfFactDiscoverer"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The diagnostic message sink.</param>
        public WpfFactDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            if (testMethod.Method.ReturnType.Name == "System.Void" &&
                testMethod.Method.GetCustomAttributes(typeof(AsyncStateMachineAttribute)).Any())
            {
                return new ExecutionErrorTestCase(this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), TestMethodDisplayOptions.None, testMethod, "Async void methods are not supported.");
            }

            return OSUtil.IsWindows()
                ? (IXunitTestCase)new UITestCase(UITestCase.SyncContextType.WPF, this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod)
                : new XunitSkippedDataRowTestCase(this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, "WPF only exists on Windows.");
        }
    }
}