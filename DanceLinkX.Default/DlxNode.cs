using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class DlxNode<TRow, TColumn> : IDlxNode<TRow, TColumn>
{
    internal DlxNode(DlxHead<TRow, TColumn, TRow> row, DlxHead<TRow, TColumn, TColumn> column)
    {
        Row = row;
        Column = column;
    }

    public DlxHead<TRow, TColumn, TRow> Row { get; }

    public DlxHead<TRow, TColumn, TColumn> Column { get; }

    public DlxNode<TRow, TColumn>? Up { get; internal set; }

    public DlxNode<TRow, TColumn>? Down { get; internal set; }

    public DlxNode<TRow, TColumn>? Left { get; internal set; }

    public DlxNode<TRow, TColumn>? Right { get; internal set; }

    IDlxHead<TRow, TColumn, TRow> IDlxNode<TRow, TColumn>.Row => Row;

    IDlxHead<TRow, TColumn, TColumn> IDlxNode<TRow, TColumn>.Column => Column;

    IDlxNode<TRow, TColumn>? IDlxNode<TRow, TColumn>.Up => Up;

    IDlxNode<TRow, TColumn>? IDlxNode<TRow, TColumn>.Down => Down;

    IDlxNode<TRow, TColumn>? IDlxNode<TRow, TColumn>.Left => Left;

    IDlxNode<TRow, TColumn>? IDlxNode<TRow, TColumn>.Right => Right;

    public override string ToString()
    {
        return $"DlxNode-({Row.Value}, {Column.Value})";
    }
}
