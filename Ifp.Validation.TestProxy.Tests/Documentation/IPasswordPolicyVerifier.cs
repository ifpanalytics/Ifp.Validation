namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public interface IPasswordPolicyVerifier
    {
        bool ConformsToPolicy(string password);
        bool IsWeakPassword(string password);
    }

    public class PasswordPolicyVerifier : IPasswordPolicyVerifier
    {
        public bool ConformsToPolicy(string password) => true;
        public bool IsWeakPassword(string password) => false;
    }
}
