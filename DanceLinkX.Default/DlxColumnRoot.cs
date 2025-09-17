using DanceLinkX.Services;

namespace DanceLinkX.DlxDefault;

public class DlxColumnRoot<TRow, TColumn> : DlxHeadRoot<TRow, TColumn, TColumn>
    where TColumn : notnull
{
    protected override DlxHead<TRow, TColumn, TColumn> CreateHead(TColumn value)
    {
        return new DlxColumn<TRow, TColumn>(value);
    }
}
