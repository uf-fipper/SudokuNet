using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class DlxRow<TRow, TColumn> : DlxHead<TRow, TColumn, TRow>
{
    internal DlxRow(TRow value)
        : base(value) { }

    public override IEnumerable<DlxNode<TRow, TColumn>> EnumerateNodes()
    {
        var node = FirstNode;
        while (node != null)
        {
            yield return node;
            node = node.Right;
        }
    }

    protected internal override void AddNode(DlxNode<TRow, TColumn> node)
    {
        {
            if (FirstNode == null)
            {
                FirstNode = node;
                LastNode = node;
            }
            else
            {
                LastNode!.Right = node;
                node.Left = LastNode;
                LastNode = node;
            }
            NodeCount++;
        }
    }

    public override string ToString()
    {
        return $"DlxRow-{Value}";
    }
}
