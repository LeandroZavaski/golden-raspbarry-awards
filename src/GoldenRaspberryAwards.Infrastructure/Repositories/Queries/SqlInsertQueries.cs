namespace GoldenRaspberryAwards.Infrastructure.Repositories.Queries
{
    public static class SqlInsertQueries
    {
        public const string InsertMovie = @"
            INSERT 
                INTO Movies (Year, Title, Studios, Producers, Winner) 
                VALUES (@Year, @Title, @Studios, @Producers, @Winner)";
    }
}
