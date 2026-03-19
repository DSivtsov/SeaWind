using Infrastructure.Mongo.ExerciseChat.Documents;
using MongoDB.Driver;

namespace Infrastructure.Mongo.ExerciseChat;

public interface IExerciseChatDb
{
    IMongoCollection<MongoThread> Threads { get; }
    IMongoCollection<MongoMessage> Messages { get; }
    IMongoCollection<MongoAttachment> Attachments { get; }
    IMongoCollection<MongoLock> Locks { get; }
}

public class ExerciseChatDb : IExerciseChatDb
{
    public IMongoCollection<MongoThread> Threads { get; }

    public IMongoCollection<MongoMessage> Messages { get; }

    public IMongoCollection<MongoAttachment> Attachments { get; }

    public IMongoCollection<MongoLock> Locks { get; }

    public ExerciseChatDb(MongoClient client, string dbName)
    {
        var db = client.GetDatabase(dbName);
        Threads = db.GetCollection<MongoThread>("threads");
        Messages = db.GetCollection<MongoMessage>("messages");
        Attachments = db.GetCollection<MongoAttachment>("attachments");
        Locks = db.GetCollection<MongoLock>("locks");
    }
}

