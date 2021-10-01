using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BLRT.SP.Services.Models.Signing
{
    [DataContract]
    public class DokobitResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(IsRequired = false, Name = "message")]
        public string Message { get; set; }
        [DataMember(IsRequired = false, Name = "errors")]
        public IEnumerable<string> Errors { get; set; }
        [DataMember(IsRequired = false, Name = "code")]
        public string Code { get; set; }
    }

    [DataContract, KnownType(typeof(DokobitResponse)), KnownType(typeof(Signer))]
    public class SigningCreateResponse : DokobitResponse
    {
        [DataMember(IsRequired = false, Name = "token")]
        public string Token { get; set; }
        [DataMember(IsRequired = false, Name = "signers")]
        public DokobitSigner[] Signers { get; set; }
    }

    [DataContract, KnownType(typeof(DokobitResponse))]
    public class SigningStatusResponse : DokobitResponse
    {
        [DataMember(IsRequired = false, Name = "name")]
        public string Name { get; set; }
        [DataMember(IsRequired = false, Name = "type")]
        public string Type { get; set; }
    }

    [DataContract, KnownType(typeof(DokobitResponse)), KnownType(typeof(DokobitSigning))]
    public class SigningListResponse : DokobitResponse
    {
        [DataMember(IsRequired = false, Name = "signings")]
        public DokobitSigning[] Signings { get; set; }
    }

    [DataContract, KnownType(typeof(DokobitResponse)), KnownType(typeof(DokobitSigner))]
    public class SigningShareResponse : DokobitResponse
    {
        [DataMember(IsRequired = false, Name = "Signers")]
        public DokobitSigner[] Signers { get; set; }
    }

    [DataContract]
    public class DokobitSigning
    {
        [DataMember(IsRequired = false, Name = "token")]
        public string Token { get; set; }
        [DataMember(IsRequired = false, Name = "status")]
        public string Status { get; set; }
        [DataMember(IsRequired = false, Name = "document_number")]
        public string DocumentNr { get; set; }
        [DataMember(IsRequired = false, Name = "last_signature_date")]
        public string LastSignatureDate { get; set; }
    }

    [DataContract]
    public class DokobitSigner
    {
        [DataMember(IsRequired = false, Name = "first_name")]
        public string FirstName { get; set; }
        [DataMember(IsRequired = false, Name = "last_name")]
        public string LastName { get; set; }
        [DataMember(IsRequired = false, Name = "email")]
        public string EMail { get; set; }
        [DataMember(IsRequired = false, Name = "role")]
        public string Role { get; set; }
        [DataMember(IsRequired = false, Name = "code")]
        public string Code { get; set; }
        [DataMember(IsRequired = false, Name = "country")]
        public string Country { get; set; }
        [DataMember(IsRequired = false, Name = "company")]
        public string Company { get; set; }
        [DataMember(IsRequired = false, Name = "position")]
        public string Position { get; set; }
        [DataMember(IsRequired = false, Name = "phone")]
        public string Phone { get; set; }
        [DataMember(IsRequired = false, Name = "token")]
        public string Token { get; set; }
    }
}