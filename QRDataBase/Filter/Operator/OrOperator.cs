namespace QRDataBase.Filter.Operator;

public class OrOperator : BaseOperator, ISearchItem
{
    public override DbOperator Operator { get; init; } = DbOperator.OR;
}