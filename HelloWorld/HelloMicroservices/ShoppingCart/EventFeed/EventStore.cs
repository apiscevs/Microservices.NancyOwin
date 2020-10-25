using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Newtonsoft.Json;

namespace ShoppingCart.EventFeed
{
    public class EventStore : IEventStore
    {
        private string connectionString =
        @"Data Source=STONE037\SQLEXPRESS;Initial Catalog=ShoppingCart;Integrated Security=True";
        private const string writeEventSql =
        @"insert into EventStore(Name, OccurredAt, Content) values (@Name, @OccurredAt, @Content)";
        public async Task Raise(string eventName, object content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.ExecuteAsync(
                    writeEventSql,
                    new
                    {
                        Name = eventName,
                        OccurredAt = DateTimeOffset.Now,
                        Content = jsonContent
                    });
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        private const string readEventsSql =
@"select * from EventStore where ID >= @Start and ID <= @End";
        public async Task<IEnumerable<Event>> GetEvents(
        long firstEventSequenceNumber,
        long lastEventSequenceNumber)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    return (await conn.QueryAsync<dynamic>(
                    readEventsSql,
                    new
                    {
                        Start = firstEventSequenceNumber,
                        End = lastEventSequenceNumber
                    }).ConfigureAwait(false))
                    .Select(row =>
                    {
                        var content = JsonConvert.DeserializeObject(row.Content);
                        return new Event(row.ID, row.OccurredAt, row.Name, content);
                    });
                }
            } catch(Exception e)
            {
                throw;
            }

        }
    }
}
