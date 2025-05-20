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

namespace QBM.CompositionApi
{
    public class PostUpdateObjectExample : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>, IApiProvider
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/updategroup/{UID_AADGroup}")
                .WithParameter("UID_AADGroup", typeof(string), isInQuery: false)
                .Handle<PostedID>("POST", async (posted, qr, ct) =>
                {

                    var strUID_AADGroup = qr.Parameters.Get<string>("UID_AADGroup");
                    var query1 = Query.From("AADGroup")
                        .Select("DisplayName", "UID_AADOrganization", "MailNickName", "Mail", "Description")
                        .Where(string.Format("UID_AADGroup='{0}'", strUID_AADGroup));

                    var DisplayName = "";
                    var UID_AADOrganization = "";
                    var MailNickName = "";
                    var Mail = "";
                    var Description = "";


                    var tryget = await qr.Session.Source()
                        .TryGetAsync(query1, EntityLoadType.DelayedLogic, ct)
                        .ConfigureAwait(false);

                    if (tryget.Success)
                    {

                        foreach (var column in posted.columns)
                        {
                            if (column.column == "DisplayName")
                            {
                                DisplayName = column.value.ToString();

                                // Check that DisplayName starts with "aad"
                                if (string.IsNullOrWhiteSpace(DisplayName) || !DisplayName.StartsWith("aad", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new ArgumentException("DisplayName must start with the prefix 'aad'.");
                                }

                                await tryget.Result.PutValueAsync("DisplayName", DisplayName, ct).ConfigureAwait(false);
                            }
                            else if (column.column == "UID_AADOrganization")
                            {
                                UID_AADOrganization = column.value.ToString();
                                await tryget.Result.PutValueAsync("UID_AADOrganization", UID_AADOrganization, ct).ConfigureAwait(false);
                            }
                            else if (column.column == "MailNickName")
                            {
                                MailNickName = column.value.ToString();
                                await tryget.Result.PutValueAsync("MailNickName", MailNickName, ct).ConfigureAwait(false);
                            }
                            else if (column.column == "Mail")
                            {
                                Mail = column.value.ToString();
                                await tryget.Result.PutValueAsync("Mail", Mail, ct).ConfigureAwait(false);
                            }
                            else if (column.column == "Description")
                            {
                                Description = column.value.ToString();
                                await tryget.Result.PutValueAsync("Description", Description, ct).ConfigureAwait(false);
                            }
                        }
                    }
                    
                    using (var u = qr.Session.StartUnitOfWork())
                        {
                        await u.PutAsync(tryget.Result, ct).ConfigureAwait(false);

                            await u.CommitAsync(ct).ConfigureAwait(false);
                        }
                    }

                ));
        }

        public class PostedID
        {
            public string index { get; set; }

            public columnsarray[] columns { get; set; }
        }

        public class columnsarray
        {
            public string column { get; set; }

            public object value { get; set; }
        }
    }
}
