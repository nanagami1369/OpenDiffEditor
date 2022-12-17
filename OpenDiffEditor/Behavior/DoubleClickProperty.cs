﻿using System.Windows.Input;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace OpenDiffEditor.Behavior;

public class DoubleClickProperty
{
    public static readonly DependencyProperty CommandProperty
        = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(DoubleClickProperty),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is not Control control) { throw new ArgumentException("control 属性のコントロールにつけてください"); }
                control.DoubleTapped -= DoubleClickCommand;
                control.DoubleTapped += DoubleClickCommand;
            })
        );

    public static void SetCommand(DependencyObject obj, ICommand value)
        => obj.SetValue(CommandProperty, value);

    public static ICommand GetCommand(DependencyObject obj)
        => (ICommand)obj.GetValue(CommandProperty);

    public static readonly DependencyProperty CommandParameterProperty
    = DependencyProperty.RegisterAttached(
        "CommandParameter",
        typeof(object),
        typeof(DoubleClickProperty),
        new PropertyMetadata(null)
    );

    public static void SetCommandParameter(DependencyObject obj, object value)
        => obj.SetValue(CommandParameterProperty, value);

    public static object GetCommandParameter(DependencyObject obj)
        => obj.GetValue(CommandParameterProperty);


    private static void DoubleClickCommand(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not DependencyObject d) { return; }
        var command = GetCommand(d);
        var parameter = GetCommandParameter(d);
        if (command?.CanExecute(parameter) ?? false)
        {
            command.Execute(parameter);
        }
    }
}
