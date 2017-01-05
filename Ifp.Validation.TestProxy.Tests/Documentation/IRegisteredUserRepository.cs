using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public interface IUser
    {

    }
    public interface IRegisteredUserRepository
    {
        IUser GetUserByEMailaddress(string emailAddress);
    }
    public class RegisteredUserRepository : IRegisteredUserRepository
    {
        public IUser GetUserByEMailaddress(string emailAddress) => null;
    }
}
