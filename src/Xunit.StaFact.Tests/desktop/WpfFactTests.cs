﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Xunit;
using DesktopFactAttribute = Xunit.WpfFactAttribute;
using DesktopSyncContext = System.Windows.Threading.DispatcherSynchronizationContext;

/// <summary>
/// Verifies behavior of the <see cref="WinFormsFactAttribute"/>.
/// </summary>
/// <remarks>
/// The members of this class should be kept in exact sync with those of the
/// <see cref="WinFormsFactTests"/> since they should behave the same way.
/// </remarks>
// ReSharper disable once CheckNamespace
public class WpfFactTests
{
    private readonly Thread ctorThread;
    // ReSharper disable once NotAccessedField.Local
    private readonly SynchronizationContext ctorSyncContext;

    public WpfFactTests()
    {
        this.ctorThread = Thread.CurrentThread;
        this.ctorSyncContext = SynchronizationContext.Current;
    }

    [DesktopFact]
    public void Void()
    {
        this.AssertThreadCharacteristics();
    }

    [DesktopFact]
    public async Task AsyncTask()
    {
        this.AssertThreadCharacteristics();
        await Task.Yield();
        this.AssertThreadCharacteristics();
    }

    [DesktopFact, Trait("Category", "FailureExpected")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async void AsyncVoid_IsNotSupported()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
    }

    [DesktopFact, Trait("Category", "FailureExpected")]
    public async Task FailAfterYield_Task()
    {
        // Task.Yield posts a message immediately (before yielding)
        await Task.Yield();
        Assert.False(true);
    }

    [DesktopFact, Trait("Category", "FailureExpected")]
    public async Task FailAfterDelay_Task()
    {
        // Task.Delay waits for the elapsed time after yielding before posting a message.
        await Task.Delay(10);
        Assert.False(true);
    }

    [DesktopFact, Trait("Category", "FailureExpected")]
    public async Task OperationCanceledException_Thrown()
    {
        await Task.Yield();
        throw new OperationCanceledException();
    }

    [DesktopFact]
    public void ShouldShowWindow()
    {
        var window = new Window();
        window.Show();

        Assert.True(window.IsVisible);
    }

    private void AssertThreadCharacteristics()
    {
        Assert.IsType<DesktopSyncContext>(SynchronizationContext.Current);

        Assert.Same(this.ctorThread, Thread.CurrentThread);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
    }
}
