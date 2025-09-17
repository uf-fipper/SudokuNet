using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public abstract class DlxHead<TRow, TColumn, T> : IDlxHead<TRow, TColumn, T>
{
    internal DlxHead(T value)
    {
        Value = value;
        Next = this;
        Prev = this;
    }

    public T Value { get; }

    public int NodeCount { get; internal set; }

    int IDlxHead<TRow, TColumn, T>.NodeCount => NodeCount;

    public DlxHead<TRow, TColumn, T> Next { get; internal set; }

    public DlxHead<TRow, TColumn, T> Prev { get; internal set; }

    public DlxNode<TRow, TColumn>? FirstNode { get; protected set; }

    public DlxNode<TRow, TColumn>? LastNode { get; protected set; }

    public bool IsCover { get; protected set; }

    IDlxHead<TRow, TColumn, T> IDlxHead<TRow, TColumn, T>.Next => Next;

    IDlxHead<TRow, TColumn, T> IDlxHead<TRow, TColumn, T>.Prev => Prev;

    IDlxNode<TRow, TColumn>? IDlxHead<TRow, TColumn, T>.FirstNode => FirstNode;

    IDlxNode<TRow, TColumn>? IDlxHead<TRow, TColumn, T>.LastNode => LastNode;

    protected internal abstract void AddNode(DlxNode<TRow, TColumn> node);

    public abstract IEnumerable<DlxNode<TRow, TColumn>> EnumerateNodes();

    IEnumerable<IDlxNode<TRow, TColumn>> IDlxHead<TRow, TColumn, T>.EnumerateNodes()
    {
        return EnumerateNodes();
    }

    public bool Cover()
    {
        if (IsCover)
            return false;
        Next.Prev = Prev;
        Prev.Next = Next;
        IsCover = true;
        return true;
    }

    public bool Recover()
    {
        if (!IsCover)
            return false;
        Next.Prev = this;
        Prev.Next = this;
        IsCover = false;
        return true;
    }
}
