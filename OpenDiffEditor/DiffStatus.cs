using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDiffEditor;

public enum DiffStatus
{
    None,
    Add,
    Delete,
    Modified,
}

public static class DiffStatusExpansion
{
    public static string ToStringLocal(this DiffStatus status)
    {
        return status switch
        {
            DiffStatus.None => "変化無し",
            DiffStatus.Add => "追加",
            DiffStatus.Delete => "削除",
            DiffStatus.Modified => "変更",
            _ => throw new ArgumentException("Unknown Status"),
        };
    }

    public static string ToIcon(this DiffStatus status)
    {
        return status switch
        {
            DiffStatus.None => "⬜",
            DiffStatus.Add => "🟨",
            DiffStatus.Delete => "🟥",
            DiffStatus.Modified => "🟩",
            _ => throw new ArgumentException("Unknown Status"),
        };
    }
}
