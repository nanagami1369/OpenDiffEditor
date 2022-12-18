using System.Windows.Input;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OpenDiffEditor.WPF.UI.Behavior;

public class EnterKeyDownProperty
{
    public static readonly DependencyProperty CommandProperty
        = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(EnterKeyDownProperty),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is not Control control) { throw new ArgumentException("control 属性のコントロールにつけてください"); }
                control.PreviewKeyDown -= PreviewKeyDownCommand;
                control.PreviewKeyDown += PreviewKeyDownCommand;
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
        typeof(EnterKeyDownProperty),
        new PropertyMetadata(null)
    );

    public static void SetCommandParameter(DependencyObject obj, object value)
        => obj.SetValue(CommandParameterProperty, value);

    public static object GetCommandParameter(DependencyObject obj)
        => obj.GetValue(CommandParameterProperty);


    private static void PreviewKeyDownCommand(object sender, KeyEventArgs e)
    {
        if (sender is not DependencyObject d) { return; }
        if (e.Key != Key.Enter) { return; }
        var command = GetCommand(d);
        var parameter = GetCommandParameter(d);
        if (command?.CanExecute(parameter) ?? false)
        {
            command.Execute(parameter);
            e.Handled = true;
            return;
        }
    }
}
