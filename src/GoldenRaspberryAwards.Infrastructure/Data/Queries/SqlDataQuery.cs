namespace GoldenRaspberryAwards.Infrastructure.Data.Queries
{
    public static class SqlDataQuery
    {
        public const string CreateTable = @"
            CREATE TABLE IF NOT EXISTS Movies (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Year INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Studios TEXT,
                Producers TEXT,
                Winner INTEGER NOT NULL DEFAULT 0
            );
            
            CREATE INDEX IF NOT EXISTS IX_Movies_Winner ON Movies(Winner);
            CREATE INDEX IF NOT EXISTS IX_Movies_Year ON Movies(Year);
        ";

        public const string DeleteTable = "DELETE FROM Movies;";
    }
}
