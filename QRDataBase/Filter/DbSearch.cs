namespace QRDataBase.Filter;

public class DbSearch : ISearchItem
{
    public List<ISearchItem> Items;
    
    public DbSearch(ISearchItem[] items)
    {
        Items = new List<ISearchItem>(items);
    }

    public override string ToString()
    {
        var txt = "";
        foreach (var item in Items)
        {
            txt += item + " ";
        }

        return txt;
    }
}