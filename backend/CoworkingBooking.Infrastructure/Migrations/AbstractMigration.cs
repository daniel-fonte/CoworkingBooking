namespace CoworkingBooking.Infraestructure.Migrations
{
    public abstract class AbstractMigration
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public abstract Task Up();
        public abstract Task Down();
    }
}