using MongoDB.Driver;

namespace Infrastructure.Mongo.SupportChat;

public interface ISupportChatDb
{
    IMongoDatabase Database { get; }
}

public class SupportChatDb : ISupportChatDb
{
    public IMongoDatabase Database { get; }

    public SupportChatDb(MongoClient client, string dbName)
    {
        Database = client.GetDatabase(dbName);
    }
}

/* class MongoMessageRepository (ISupportChatDb db)
     Использование:
        _messages = db.Database.GetCollection<MongoChatMessage>("messages");
 */
