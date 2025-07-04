namespace RBAC_API.Common
{
    public class ValidationResult
    {
        public List<string> Errors { get; } = new List<string>();
        public bool IsValid => !Errors.Any();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }
    }
}
