using System.ComponentModel;

namespace Domain.Enums
{
    public enum ETokenType
    {
        [Description("RegisterUser")]
        RegisterUser,
        [Description("ReferenceToken")]
        ReferenceToken,
        [Description("ForgetPassword")]
        ForgetPassword
    }
}
