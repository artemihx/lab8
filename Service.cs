using cafeapp1.Models;

namespace cafeapp1;

public class Service
{
    private static PostgresContext? _db;

    public static PostgresContext GetContext()
    {
        if (_db == null)
        {
            _db = new PostgresContext();
        }
        return _db;
    }
}