namespace DanceLinkX.Services;

public interface IDlxHead<TRow, TColumn, T>
{
    T Value { get; }

    int NodeCount { get; }

    IDlxHead<TRow, TColumn, T> Next { get; }

    IDlxHead<TRow, TColumn, T> Prev { get; }

    IDlxNode<TRow, TColumn>? FirstNode { get; }

    IDlxNode<TRow, TColumn>? LastNode { get; }

    bool IsCover { get; }

    bool Cover();

    bool Recover();

    IEnumerable<IDlxNode<TRow, TColumn>> EnumerateNodes();
}
