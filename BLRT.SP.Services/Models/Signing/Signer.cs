using System.Runtime.Serialization;

namespace BLRT.SP.Services.Models.Signing
{
    [DataContract]
    public class Signer
    {
        [DataMember(IsRequired = false)]
        public string FirstName { get; set; }
        [DataMember(IsRequired = false)]
        public string LastName { get; set; }
        [DataMember(IsRequired = false)]
        public string EMail { get; set; }
        [DataMember(IsRequired = false)]
        public string Role { get; set; }
        [DataMember(IsRequired = false)]
        public string Code { get; set; }
        [DataMember(IsRequired = false)]
        public string Country { get; set; }
        [DataMember(IsRequired = false)]
        public string Company { get; set; }
        [DataMember(IsRequired = false)]
        public string Position { get; set; }
        [DataMember(IsRequired = false)]
        public string Phone { get; set; }
        [DataMember(IsRequired = false)]
        public string Token { get; set; }
    }
}