﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable once CheckNamespace
public class Samples
{
    /// <summary>
    /// Demonstrates that the <see cref="UIFactAttribute"/> can run on all platforms,
    /// and emulates a UI thread (not specific to WPF or WinForms) with a <see cref="SynchronizationContext"/>
    /// that keeps yielding awaits on the original thread.
    /// </summary>
    [UIFact]
    public async Task UIFact_OnSTAThread()
    {
        int initialThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(SynchronizationContext.Current);
        var expectedApartment = OSUtil.IsWindows() ? ApartmentState.STA : ApartmentState.Unknown;
        Assert.Equal(expectedApartment, Thread.CurrentThread.GetApartmentState());

        await Task.Yield();
        Assert.Equal(initialThread, Environment.CurrentManagedThreadId);
        Assert.NotNull(SynchronizationContext.Current);
    }

    /// <summary>
    /// Demonstrates that xunit [Fact] behavior is to invoke the test on an MTA thread.
    /// </summary>
    [Fact]
    public async Task Fact_OnMTAThread()
    {
        var expectedApartment = OSUtil.IsWindows() ? ApartmentState.MTA : ApartmentState.Unknown;
        Assert.Equal(expectedApartment, Thread.CurrentThread.GetApartmentState());
        await Task.Yield();
        Assert.Equal(expectedApartment, Thread.CurrentThread.GetApartmentState());
    }

#if NETFRAMEWORK || NETCOREAPP3_0

    /// <summary>
    /// Demonstrates that <see cref="WpfFactAttribute"/> invokes tests expecting an STA thread
    /// specifically with a WPF SynchronizationContext.
    /// </summary>
    [WpfFact]
    public async Task WpfFact_OnSTAThread()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        Assert.IsType<System.Windows.Threading.DispatcherSynchronizationContext>(SynchronizationContext.Current);

        await Task.Yield();

        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<System.Windows.Threading.DispatcherSynchronizationContext>(SynchronizationContext.Current);
    }

    /// <summary>
    /// Demonstrates that <see cref="WinFormsFactAttribute"/> invokes tests expecting an STA thread
    /// specifically with a WinForms SynchronizationContext.
    /// </summary>
    [WinFormsFact]
    public async Task WinFormsFact_OnSTAThread()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        Assert.IsType<System.Windows.Forms.WindowsFormsSynchronizationContext>(SynchronizationContext.Current);

        await Task.Yield();

        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<System.Windows.Forms.WindowsFormsSynchronizationContext>(SynchronizationContext.Current);
    }

    /// <summary>
    /// Demonstrates that <see cref="StaFactAttribute"/> invokes tests expecting an STA thread
    /// but with no <see cref="SynchronizationContext"/> at all, so any yielding await will return on the threadpool (an MTA thread).
    /// </summary>
    [StaFact]
    public async Task StaWithoutSyncContext()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        Assert.Null(SynchronizationContext.Current);

        await Task.Yield();

        // Without a single-threaded SynchronizationContext, we won't come back to the STA thread.
        Assert.Equal(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
        Assert.Null(SynchronizationContext.Current);
    }

#endif
}
