namespace efc.Models;

public class FeatureFlag(string Name, bool IsEnabled, DateTime CreatedOnUtc, DateTime? EnabledOn, DateTime? UpdatedOnUtc, List<FeatureFlagUserAssignment>? UserAssignments)
{
    public string Name { get; private set; } = Name;
    public bool IsEnabled { get; private set; } = IsEnabled;
    public DateTime CreatedOnUtc { get; private set; } = CreatedOnUtc;
    public DateTime? EnabledOn { get; private set; } = EnabledOn;
    public DateTime? UpdatedOnUtc { get; private set; } = UpdatedOnUtc;
    public List<FeatureFlagUserAssignment> UserAssignments { get; private set; } = UserAssignments ?? [];

    public IEnumerable<Exception> TryValidate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new Exception("Name is required.");
        }
    }

    public void Validate()
    {
        var exceptions = TryValidate().ToList();
        if (exceptions.Any())
        {
            throw new AggregateException(exceptions);
        }
    }
}
