using System.ComponentModel.DataAnnotations;

namespace FITNESS_FACTION_.NET_CORE_CONVERSIONS.Authorization
{
    public class UserIdentity
    {
        [StringLength(40)]
        public string UserName { get; set; }
    }
}
