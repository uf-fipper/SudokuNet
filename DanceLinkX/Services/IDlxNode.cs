namespace DanceLinkX.Services;

public interface IDlxNode<TRow, TColumn>
{
    IDlxHead<TRow, TColumn, TRow> Row { get; }

    IDlxHead<TRow, TColumn, TColumn> Column { get; }

    IDlxNode<TRow, TColumn>? Up { get; }

    IDlxNode<TRow, TColumn>? Down { get; }

    IDlxNode<TRow, TColumn>? Left { get; }

    IDlxNode<TRow, TColumn>? Right { get; }
}
