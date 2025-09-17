using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public abstract class DlxHeadRoot<TRow, TColumn, T>
    : DlxHead<TRow, TColumn, T>,
        IDlxHeadRoot<TRow, TColumn, T>
    where T : notnull
{
    internal DlxHeadRoot()
        : base(default!) { }

    protected internal override void AddNode(DlxNode<TRow, TColumn> node)
    {
        throw new InvalidOperationException();
    }

    public override IEnumerable<DlxNode<TRow, TColumn>> EnumerateNodes()
    {
        yield break;
    }

    internal readonly Dictionary<T, DlxHead<TRow, TColumn, T>> HeadsMap = [];

    public DlxHead<TRow, TColumn, T>? GetHeadByValue(T value)
    {
        HeadsMap.TryGetValue(value, out var column);
        return column;
    }

    internal bool TryAddHead(T value, out DlxHead<TRow, TColumn, T> head)
    {
        if (HeadsMap.TryGetValue(value, out var c))
        {
            head = c;
            return false;
        }
        head = CreateHead(value);
        Prev.Next = head;
        head.Prev = Prev;
        head.Next = this;
        Prev = head;
        HeadsMap.Add(value, head);
        return true;
    }

    protected abstract DlxHead<TRow, TColumn, T> CreateHead(T value);

    public IEnumerable<DlxHead<TRow, TColumn, T>> EnumerateHeads()
    {
        var col = Next;
        while (col != this)
        {
            yield return col;
            col = col.Next;
        }
    }

    public bool IsEmpty()
    {
        return Next == this;
    }

    IEnumerable<IDlxHead<TRow, TColumn, T>> IDlxHeadRoot<TRow, TColumn, T>.EnumerateHeads()
    {
        return EnumerateHeads();
    }

    IDlxHead<TRow, TColumn, T>? IDlxHeadRoot<TRow, TColumn, T>.GetHeadByValue(T value)
    {
        return GetHeadByValue(value);
    }
}
