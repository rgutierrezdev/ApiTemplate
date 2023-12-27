namespace ApiTemplate.Domain.Common;

/**
 * This custom implementation was created specifically to store sequential UUIDs in SQL Server
 * because SQL Server does not compare ids size as strings but using groups of bytes
 * see: https://web.archive.org/web/20120628234912/http://blogs.msdn.com/b/sqlprogrammability/archive/2006/11/06/how-are-guids-compared-in-sql-server-2005.aspx
 */
public abstract class Ulid
{
    private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

    // // This is basically a migration of a T-SQL implementation in https://github.com/rmalayter/ulid-mssql/tree/master
    // // But it was not working properly leading to a high percent of fragmentation
    // public static Guid NewGuid()
    // {
    //     var randomBytes = RandomNumberGenerator.GetBytes(10);
    //     var utcNow = DateTimeOffset.UtcNow;
    //     var diff = utcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    //
    //     var timeBinary = BitConverter.GetBytes((ulong)diff);
    //
    //     var combined = new byte[16];
    //     Array.Copy(randomBytes, combined, randomBytes.Length);
    //     Array.Copy(timeBinary, 2, combined, randomBytes.Length, 6);
    //
    //     return new Guid(combined);
    // }

    /**
     * This one was taken from NHibernate
     * see: https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs
     */
    public static Guid NewGuid()
    {
        var guidArray = Guid.NewGuid().ToByteArray();

        var now = DateTime.UtcNow;

        // Get the days and milliseconds which will be used to build the byte string
        var days = new TimeSpan(now.Ticks - BaseDateTicks);
        var msecs = now.TimeOfDay;

        // Convert to a byte array
        // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
        var daysArray = BitConverter.GetBytes(days.Days);
        var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering
        Array.Reverse(daysArray);
        Array.Reverse(msecsArray);

        // Copy the bytes into the guid
        Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
        Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new Guid(guidArray);
    }
}
