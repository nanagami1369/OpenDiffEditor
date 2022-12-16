using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenDiffEditor.Behavior
{
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
                control.PreviewDragOver -= PreviewDragOver;

                control.AllowDrop = e.NewValue is ICommand;
                if (control.AllowDrop)
                {
                    control.DragOver += DragOver;
                    control.Drop += Drop;
                    // TextBoxは、Dropだと動かないのでPreviewDragOverを使う
                    if (d is TextBox textBox)
                    {
                        textBox.PreviewDragOver += PreviewDragOver;
                    }
                }
            }));

        private static void PreviewDragOver(object sender, DragEventArgs e)
        {
            DragOver(sender, e);
            e.Handled = true;
        }

        private static void DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            };
        }

        private static void Drop(object sender, DragEventArgs e)
        {
            if (
                e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop) is string[] paths &&
                paths.FirstOrDefault() is string path &&
                sender is DependencyObject d &&
                GetCommand(d) is ICommand command &&
                command.CanExecute(path)
                )
            {
                command.Execute(path);
            };
        }
    }
}
