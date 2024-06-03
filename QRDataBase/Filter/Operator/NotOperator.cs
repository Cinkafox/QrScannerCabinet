namespace QRDataBase.Filter.Operator;

public class NotOperator : BaseOperator, ISearchItem
{
    public override DbOperator Operator { get; init; } = DbOperator.NOT;
}