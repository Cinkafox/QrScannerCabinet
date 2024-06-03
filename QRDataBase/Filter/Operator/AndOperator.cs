namespace QRDataBase.Filter.Operator;

public class AndOperator : BaseOperator, ISearchItem
{
    public override DbOperator Operator { get; init; } = DbOperator.AND;
}