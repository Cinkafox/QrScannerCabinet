namespace QRDataBase.Filter.Operator;

public abstract class BaseOperator : IDbOperator, ISearchItem
{
    public abstract DbOperator Operator { get; init; }
    public override string ToString()
    {
        return Operator.ToString();
    }
}