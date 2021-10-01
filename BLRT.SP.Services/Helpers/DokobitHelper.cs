using BLRT.SP.Services.Models.Signing;
using Microsoft.SharePoint.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace BLRT.SP.Services.Helpers
{
    public class DokobitHelper
    {
        public static string accessToken = ConfigurationManager.AppSettings["DokobitAccessToken"].ToString(); //"beta_fbeb77631c3fd82067adf7a275aa362e0283b6f5";

        public static Stream DownloadSigning(string token)
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(Endpoint.SigningDownload(token, accessToken)))
                {
                    var input = response.Result;
                    input.EnsureSuccessStatusCode();
                    return input.Content.ReadAsStreamAsync().Result;
                }
            }
        }

        public static SigningStatusResponse CheckSigningStatus(string token)
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(Endpoint.SigningStatus(token, accessToken)))
                {
                    var input = response.Result;
                    input.EnsureSuccessStatusCode();
                    var serializator = new DataContractJsonSerializer(typeof(SigningStatusResponse));
                    return (SigningStatusResponse)serializator.ReadObject(input.Content.ReadAsStreamAsync().Result);
                }
            }
        }

        public static SigningCreateResponse CreateSigning(CreateSigningParams signingParams)
        {
            var fileIdx = 0;
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now))
                {
                    content.Add(new StringContent(signingParams.Type), "type");
                    content.Add(new StringContent(signingParams.Title), "name");

                    using (ClientContext context = AuthHelper.GetClientContext(signingParams.WebUrl))
                    {
                        List list = context.Web.Lists.GetById(new Guid(signingParams.ListId));
                        Microsoft.SharePoint.Client.File main = list.GetItemById(signingParams.MainItemId).File;
                        context.Load(main, a => a.Name);
                        byte[] fileArr = SharepointHelper.ConvertFileToByteArray(context, main);
                        content.Add(new StringContent(main.Name), string.Format("files[{0}][name]", fileIdx));
                        content.Add(new StringContent("main"), string.Format("files[{0}][type]", fileIdx));
                        content.Add(new StringContent(Convert.ToBase64String(fileArr)), string.Format("files[{0}][content]", fileIdx));
                        content.Add(
                            new StringContent(
                                BitConverter.ToString(SHA256.Create().ComputeHash(fileArr)).Replace("-", "").ToLower()),
                            string.Format("files[{0}][digest]", fileIdx));
                        fileIdx++;
                        for (int i = 0; i < signingParams.AttachmentItemIds.Length; i++)
                        {
                            var attachmentId = signingParams.AttachmentItemIds[i];
                            Microsoft.SharePoint.Client.File attachment = list.GetItemById(attachmentId).File;
                            context.Load(attachment, a => a.Name);
                            byte[] attachmentArr = SharepointHelper.ConvertFileToByteArray(context, attachment);

                            content.Add(new StringContent(attachment.Name), string.Format("files[{0}][name]", fileIdx));
                            content.Add(new StringContent("attachment"), string.Format("files[{0}][type]", fileIdx));
                            content.Add(new StringContent(Convert.ToBase64String(attachmentArr)), string.Format("files[{0}][content]", fileIdx));
                            content.Add(
                                new StringContent(
                                    BitConverter.ToString(SHA256.Create().ComputeHash(attachmentArr)).Replace("-", "").ToLower()),
                                string.Format("files[{0}][digest]", fileIdx));
                            fileIdx++;
                        }
                        for (int i = 0; i < signingParams.AppendixItemIds.Length; i++)
                        {
                            var appendixId = signingParams.AppendixItemIds[i];
                            Microsoft.SharePoint.Client.File appendix = list.GetItemById(appendixId).File;
                            context.Load(appendix, a => a.Name);
                            byte[] appendixArr = SharepointHelper.ConvertFileToByteArray(context, appendix);
                            content.Add(new StringContent(appendix.Name), string.Format("files[{0}][name]", fileIdx));
                            content.Add(new StringContent("appendix"), string.Format("files[{0}][type]", fileIdx));
                            content.Add(new StringContent(Convert.ToBase64String(appendixArr)), string.Format("files[{0}][content]", fileIdx));
                            content.Add(
                                new StringContent(
                                    BitConverter.ToString(SHA256.Create().ComputeHash(appendixArr)).Replace("-", "").ToLower()),
                                string.Format("files[{0}][digest]", fileIdx));
                            fileIdx++;
                        }


                    }

                    var signers = signingParams.Signers;
                    for (int i = 0; i < signers.Length; i++)
                    {
                        var signer = signers[i];
                        if (!string.IsNullOrEmpty(signer.FirstName))
                        {
                            content.Add(new StringContent(signer.FirstName), string.Format("signers[{0}][name]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.LastName))
                        {
                            content.Add(new StringContent(signer.LastName), string.Format("signers[{0}][surname]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.EMail))
                        {
                            content.Add(new StringContent(signer.EMail), string.Format("signers[{0}][email]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.Code))
                        {
                            content.Add(new StringContent(signer.Code), string.Format("signers[{0}][code]", i));
                            content.Add(new StringContent(signer.Country), string.Format("signers[{0}][country_code]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.Phone))
                        {
                            content.Add(new StringContent(signer.Phone), string.Format("signers[{0}][phone]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.Role))
                        {
                            content.Add(new StringContent(signer.Role), string.Format("signers[{0}][role]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.Company))
                        {
                            content.Add(new StringContent(signer.Company), string.Format("signers[{0}][company]", i));
                        }
                        if (!string.IsNullOrEmpty(signer.Position))
                        {
                            content.Add(new StringContent(signer.Position), string.Format("signers[{0}][position]", i));
                        }
                    }
                    content.Add(new StringContent(signingParams.RequireQualifiedSignatures.ToString()), "require_qualified_signatures");
                    if (!string.IsNullOrEmpty(signingParams.PostbackUrl)) { content.Add(new StringContent(signingParams.PostbackUrl), "postback_url"); }

                    using (
                         var message =
                             client.PostAsync(Endpoint.SigningCreate(DokobitHelper.accessToken),
                                 content))
                    {
                      
                        var input = message.Result;  
                        input.EnsureSuccessStatusCode();
                        var serializator = new DataContractJsonSerializer(typeof(SigningCreateResponse));
                        return (SigningCreateResponse)serializator.ReadObject(input.Content.ReadAsStreamAsync().Result);
                    }
                }
            }
        }

        public static SigningListResponse ListSignings()
        {
            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(Endpoint.List(accessToken)))
                {
                    var input = response.Result;
                    input.EnsureSuccessStatusCode();
                    var serializator = new DataContractJsonSerializer(typeof(SigningListResponse));
                    return (SigningListResponse)serializator.ReadObject(input.Content.ReadAsStreamAsync().Result);
                }
            }
        }

        public class Endpoint
        {
            public static string endpoint = ConfigurationManager.AppSettings["DokobitEndpoint"].ToString();// "https://beta.dokobit.com/api/";

            public static string SigningCreate(string accessToken)
            {
                return endpoint + "signing/create.json?access_token=" + accessToken;
            }

            public static string List(string accessToken)
            {
                return endpoint + "signing/list.json?access_token=" + accessToken;
            }

            public static string SigningStatus(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/status.json?access_token=" + accessToken;
            }

            public static string SigningDownload(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/download?access_token=" + accessToken;
            }

            public static string SigningDelete(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/delete?access_token=" + accessToken;

            }

            public static string DeleteAll(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/delete-all?access_token=" + accessToken;
            }

            public static string SigningShare(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/share?access_token=" + accessToken;
            }

            public static string RemoveSigner(string token, string accessToken)
            {
                return endpoint + "signing/" + token + "/signers/delete?access_token=" + accessToken;
            }

        }
    }
}