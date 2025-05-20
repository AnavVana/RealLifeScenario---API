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
using System.Runtime.ConstrainedExecution;

namespace QBM.CompositionApi
{
    public class PostGroup : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>, IApiProvider
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/postgroup")
                .Handle<PostedID, IEntity>("POST", async (posted, qr, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(posted.DisplayName) || !posted.DisplayName.StartsWith("aad", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("DisplayName must start with the prefix 'aad'.");
                    }

                    // Create a new 'AADGroup' entity
                    var newID = await qr.Session.Source().CreateNewAsync("AADGroup",
                        new EntityParameters
                        {
                            CreationType = EntityCreationType.DelayedLogic
                        }, ct).ConfigureAwait(false);

                    await newID.PutValueAsync("DisplayName", posted.DisplayName, ct).ConfigureAwait(false);
                    await newID.PutValueAsync("UID_AADOrganization", posted.UID_AADOrganization, ct).ConfigureAwait(false);
                    await newID.PutValueAsync("MailNickName", posted.MailNickName, ct).ConfigureAwait(false);
                    await newID.PutValueAsync("Mail", posted.Mail, ct).ConfigureAwait(false);
                    await newID.PutValueAsync("Description", posted.Description, ct).ConfigureAwait(false);


                    using (var u = qr.Session.StartUnitOfWork())
                    {
                        await u.PutAsync(newID, ct).ConfigureAwait(false);
                        await u.CommitAsync(ct).ConfigureAwait(false);
                    }

                    return newID;

                }));
        }

        public class PostedID
        {
            public string DisplayName { get; set; }
            public string UID_AADOrganization { get; set; }
            public string MailNickName { get; set; }
            public string Mail { get; set; }
            public string Description { get; set; }

        }
    }
}


