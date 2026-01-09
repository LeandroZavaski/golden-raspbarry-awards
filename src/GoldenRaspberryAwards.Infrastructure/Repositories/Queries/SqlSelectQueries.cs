namespace GoldenRaspberryAwards.Infrastructure.Repositories.Queries
{
    public static class SqlSelectQueries
    {
        public const string GetWinners = @"
            SELECT 
                Id, 
                Year, 
                Title, 
                Studios, 
                Producers, 
                Winner 
            FROM Movies 
            WHERE Winner = 1 
            ORDER BY Year";

        public const string GetAllMovies = @"
            SELECT 
                Id, 
                Year, 
                Title, 
                Studios, 
                Producers, 
                Winner 
            FROM Movies";

        public const string GetAny = @"
            SELECT 
                COUNT(1) 
            FROM Movies";
    }
}
