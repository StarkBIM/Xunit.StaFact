// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xunit;

#pragma warning disable xUnit1008

// ReSharper disable once CheckNamespace
public class WinFormsTheoryTests
{
    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task WinFormsTheory_OnSTAThread(int arg)
    {
        Assert.IsType<WindowsFormsSynchronizationContext>(SynchronizationContext.Current);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        await Task.Yield();
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<WindowsFormsSynchronizationContext>(SynchronizationContext.Current);
        Assert.True(arg == 0 || arg == 1);
    }

    [Trait("Category", "FailureExpected")]
    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task WinFormsTheoryFails(int arg)
    {
        Assert.IsType<WindowsFormsSynchronizationContext>(SynchronizationContext.Current);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        await Task.Yield();
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        Assert.IsType<WindowsFormsSynchronizationContext>(SynchronizationContext.Current);
        Assert.False(arg == 0 || arg == 1);
    }
}
