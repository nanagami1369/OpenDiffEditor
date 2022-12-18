namespace OpenDiffEditor.Common.Model;

public interface IRootPathInfo
{
    public RootPathType Type { get; }
    public string RootPath { get; }

    string GetFilePathThatEditorCanOpen(string path);

    byte[] ReadAllBytes(string path);

    bool Exist(string path);
}
