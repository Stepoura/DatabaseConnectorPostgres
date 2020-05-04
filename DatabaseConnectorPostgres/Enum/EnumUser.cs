using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnectorPostgres.Enum
{
    public enum EnumUser
    {
        SUCCESS,
        USER_EXISTS,
        FAILED,
        USER_NOT_FOUND,
        MULTIPLE_USER_FOUND,
        WRONG_PASSWORD
    }
}
