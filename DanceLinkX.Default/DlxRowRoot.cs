using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class DlxRowRoot<TRow, TColumn> : DlxHeadRoot<TRow, TColumn, TRow>
    where TRow : notnull
{
    protected override DlxHead<TRow, TColumn, TRow> CreateHead(TRow value)
    {
        return new DlxRow<TRow, TColumn>(value);
    }
}
