// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

using System.Threading;

using Xunit;

// ReSharper disable once CheckNamespace
public class StaFactCtorSyncContextTests
{
    private readonly SynchronizationContext ctorSyncContext;

    public StaFactCtorSyncContextTests()
    {
        this.ctorSyncContext = new SynchronizationContext();
        Assert.Null(SynchronizationContext.Current);
        SynchronizationContext.SetSynchronizationContext(this.ctorSyncContext);
    }

    [StaFact]
    public void SyncContextPreservedFromCtor()
    {
        Assert.Same(this.ctorSyncContext, SynchronizationContext.Current);
    }
}
