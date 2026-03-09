using MongoDB.Driver;

namespace Infrastructure.Mongo.ExerciseChat;

public interface IExerciseChatDb
{
    IMongoDatabase Database { get; }
}

public class ExerciseChatDb : IExerciseChatDb
{
    public IMongoDatabase Database { get; }

    public ExerciseChatDb(MongoClient client, string dbName)
    {
        Database = client.GetDatabase(dbName);
    }
}

