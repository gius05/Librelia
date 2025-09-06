using MongoDB.Driver;

namespace Librelia.Repositories
{
    public class SettingsRepo
    {
        public readonly FindOptions _findOptions = new FindOptions() { Collation = new Collation("it", strength: CollationStrength.Secondary) };
    }
}
