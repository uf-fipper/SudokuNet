using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class DlxColumn<TRow, TColumn> : DlxHead<TRow, TColumn, TColumn>
{
    internal DlxColumn(TColumn value)
        : base(value) { }

    public override IEnumerable<DlxNode<TRow, TColumn>> EnumerateNodes()
    {
        var node = FirstNode;
        while (node != null)
        {
            yield return node;
            node = node.Down;
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
                LastNode!.Down = node;
                node.Up = LastNode;
                LastNode = node;
            }
            NodeCount++;
        }
    }

    public override string ToString()
    {
        return $"DlxColumn-{Value}";
    }
}
