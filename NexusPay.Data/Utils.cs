namespace NexusPay.Data
{
    public static class Utils
    {
        public static object DBNullParse(object? parameter)
        {
            if (parameter is null)
            {
                return DBNull.Value;
            }

            return parameter;
        }
    }
}
