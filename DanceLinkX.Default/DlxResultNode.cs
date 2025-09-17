using System.Diagnostics.CodeAnalysis;

namespace DanceLinkX.DlxDefault;

public class DlxResultNode<TRow>(TRow? row)
    where TRow : notnull
{
    public TRow? Row { get; } = row;
    public bool IsValid { get; private set; }
    public bool IsLeaf { get; private set; }
    public List<DlxResultNode<TRow>> Children { get; private set; } = [];
    public DlxResultNode<TRow>? Parent { get; private set; }

    internal void AddChild(DlxResultNode<TRow> node)
    {
        Children.Add(node);
        node.Parent = this;
    }

    internal void SetLeaf()
    {
        IsLeaf = true;
    }

    internal void SetValid()
    {
        var node = this;
        while (node != null && !node.IsValid)
        {
            node.IsValid = true;
            node = node.Parent;
        }
    }

    public List<TRow> GetRowList()
    {
        var now = this;
        List<TRow> result = [];
        while (now!.Parent != null)
        {
            result.Add(now.Row!);
            now = now.Parent;
        }
        result.Reverse();
        return result;
    }
}
