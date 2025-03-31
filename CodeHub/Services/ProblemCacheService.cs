using CodeHub.Data.Models;

public class ProblemCacheService
{
    public List<Problem> CachedProblems { get; private set; } = new();
    public bool IsLoaded { get; private set; } = false;

    public void SetProblems(List<Problem> problems)
    {
        CachedProblems = problems;
        IsLoaded = true;
    }

    public List<Problem> GetProblems() => CachedProblems;
}