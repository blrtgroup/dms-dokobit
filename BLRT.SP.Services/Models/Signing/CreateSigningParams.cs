using System;
using System.Runtime.Serialization;

namespace BLRT.SP.Services.Models.Signing
{
    [DataContract]
    [KnownType(typeof(ItemIdentifier))]
    [KnownType(typeof(Signer))]
    public class CreateSigningParams : ItemIdentifier
    {
        [DataMember(IsRequired = true)]
        public string Title { get; set; }
        [DataMember(IsRequired = true)]
        public string Type { get; set; }
        [DataMember(IsRequired = false)]
        public string[] Categories { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public DateTime? Deadline { get; set; }
        [DataMember]
        public bool RequireQualifiedSignatures { get; set; }
        [DataMember(IsRequired = false)]
        public string PostbackUrl { get; set; }
        [DataMember(IsRequired = true)]
        public Signer[] Signers { get; set; }
        [DataMember(IsRequired = true)]
        public int MainItemId { get; set; }
        [DataMember(IsRequired = false)]
        public int[] AppendixItemIds { get; set; }
        [DataMember(IsRequired = false)]
        public int[] AttachmentItemIds { get; set; }
    }
}