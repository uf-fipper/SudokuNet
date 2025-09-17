namespace DanceLinkX.Services;

public interface IDlxHeadRoot<TRow, TColumn, T> : IDlxHead<TRow, TColumn, T>
{
    bool IsEmpty();

    IEnumerable<IDlxHead<TRow, TColumn, T>> EnumerateHeads();

    IDlxHead<TRow, TColumn, T>? GetHeadByValue(T value);

    T IDlxHead<TRow, TColumn, T>.Value => default!;

    IDlxNode<TRow, TColumn>? IDlxHead<TRow, TColumn, T>.FirstNode => null;

    IDlxNode<TRow, TColumn>? IDlxHead<TRow, TColumn, T>.LastNode => null;

    int IDlxHead<TRow, TColumn, T>.NodeCount => 0;

    bool IDlxHead<TRow, TColumn, T>.IsCover => false;

    bool IDlxHead<TRow, TColumn, T>.Cover() => throw new NotSupportedException();

    bool IDlxHead<TRow, TColumn, T>.Recover() => throw new NotSupportedException();
}
