using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace OpenDiffEditor.Behavior;

public class FilePathDropAttachedProperty
{


    public static ICommand GetCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(CommandProperty, value);
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(FilePathDropAttachedProperty), new PropertyMetadata(null, (d, e) =>
        {
            if (d is not Control control) { throw new ArgumentException("control 属性のコントロールにつけてください"); }
            control.DragOver -= DragOver;
            control.Drop -= Drop;

            control.AllowDrop = e.NewValue is ICommand;
            if (control.AllowDrop)
            {
                control.DragOver += DragOver;
                control.Drop += Drop;
            }
        }));

    private static void DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }

    private static async void Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            var path = items.FirstOrDefault()?.Path;
            if (path is not null && sender is DependencyObject d && GetCommand(d) is ICommand command && command.CanExecute(path))
            {
                command.Execute(path);
            };
        }
    }
}
