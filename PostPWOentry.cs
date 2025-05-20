using System;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using VI.Base;
using VI.DB;
using VI.DB.Entities;
using QBM.CompositionApi.ApiManager;
using QBM.CompositionApi.Definition;
using QBM.CompositionApi.Crud;
using QER.CompositionApi.Portal;
using System.Xml.Linq;

namespace QBM.CompositionApi
{
    public class PostPWOentry : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>, IApiProvider
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/postpwoentry")
                    .Handle<PostedID, string>("POST", async (posted, qr, ct) =>
                    {
                        // Variables to hold column data from the posted request
                        var uid_personInserted = qr.Session.User().Uid;
                        var ObjectKeyOrdered = "";
                        string uid_personOrdered = "";
                        string uid_org = "";
                        string aad_username = posted.aad_username;
                        string uid_aad_group = posted.uid_aad_group;
                        
                        // Query to find the UID_Person is about to assigned the AADGroup
                        var q1 = Query.From("AADUser")
                            .Select("UID_Person")
                            .Where(string.Format("UserPrincipalName = '{0}'", aad_username));
                        var tryGetUidPerson = await qr.Session.Source().TryGetAsync(q1, EntityLoadType.DelayedLogic).ConfigureAwait(false);

                        var q2 = Query.From("ITShopOrg")
                            .Select("UID_ITShopOrg")
                            .Where(string.Format("UID_AccProduct in (select UID_AccProduct from AADGroup where UID_AADGroup = '{0}')", uid_aad_group));
                        var tryGetUidProduct = await qr.Session.Source().TryGetAsync(q2, EntityLoadType.DelayedLogic).ConfigureAwait(false);

                        var q3 = Query.From("AADGroup")
                            .Select("XObjectKey")
                            .Where(string.Format("UID_AADGroup = '{0}'", uid_aad_group));
                        var tryGetObjectKey = await qr.Session.Source().TryGetAsync(q3, EntityLoadType.DelayedLogic).ConfigureAwait(false);


                        // store the UID_Org's UID
                        if (tryGetUidPerson.Success && tryGetUidProduct.Success && tryGetObjectKey.Success)
                        {
                            uid_personOrdered = tryGetUidPerson.Result.GetValue<string>("UID_Person");
                            uid_org = tryGetUidProduct.Result.GetValue<string>("UID_ITShopOrg");
                            ObjectKeyOrdered = tryGetObjectKey.Result.GetValue<string>("XObjectKey");

                            // Create a new entity
                            var newID = await qr.Session.Source().CreateNewAsync("PersonWantsOrg",
                            new EntityParameters
                            {
                                CreationType = EntityCreationType.DelayedLogic
                            }, ct).ConfigureAwait(false);

                            // Set the values for the new entity
                            await newID.PutValueAsync("UID_PersonInserted", uid_personInserted, ct).ConfigureAwait(false);
                            await newID.PutValueAsync("UID_PersonOrdered", uid_personOrdered, ct).ConfigureAwait(false);
                            await newID.PutValueAsync("UID_Org", uid_org, ct).ConfigureAwait(false);
                            await newID.PutValueAsync("ObjectKeyOrdered", ObjectKeyOrdered, ct).ConfigureAwait(false);

                            using (var u = qr.Session.StartUnitOfWork())
                            {
                                await u.PutAsync(newID, ct).ConfigureAwait(false);
                                await u.CommitAsync(ct).ConfigureAwait(false);
                            }

                            return $"Assignment of AAD Group '{uid_aad_group}' to '{posted.aad_username}' completed.";
                        }
                        else
                        {
                            return $"AAD Group '{uid_aad_group}' assignment to user '{posted.aad_username}' failed.";
                        }; 
                    }));
        }

        public class PostedID
        {
            public string aad_username { get; set; }
            public string uid_aad_group { get; set; }
        }
    } 
}
