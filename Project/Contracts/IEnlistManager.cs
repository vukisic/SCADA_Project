using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IEnlistManager
    {
        void Enlist();

        void EndEnlist(bool isSuccessful);
    }
}
