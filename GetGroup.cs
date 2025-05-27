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
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

namespace QBM.CompositionApi // theBranch comment
{
    public class GetGroup : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject> 
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/getgroup/{UID_AADGroup}")
                .WithParameter("UID_AADGroup", typeof(string), isInQuery:false)
                .HandleGet(async (qr, ct) =>
                    {
                        var strUID_AADGroup = qr.Parameters.Get<string>("UID_AADGroup");

                        var query1 = Query.From("AADGroup")
                        .Select("DisplayName", "UID_AADOrganization", "MailNickName", "Mail", "Description")
                        .Where(string.Format("UID_AADGroup='{0}'", strUID_AADGroup));
                        

                        var tryGet = await qr.Session.Source()
                            .TryGetAsync(query1, EntityLoadType.DelayedLogic)
                            .ConfigureAwait(false);
                        if (tryGet.Success) {
                            var myObject = new Dictionary<string, string>
                            {
                                {"DisplayName" , await tryGet.Result.GetValueAsync<string>("DisplayName").ConfigureAwait(false) },
                                {"UID_AADOrganization" , await tryGet.Result.GetValueAsync<string>("UID_AADOrganization").ConfigureAwait(false) },
                                {"MailNickName" , await tryGet.Result.GetValueAsync<string>("MailNickName").ConfigureAwait(false) },
                                {"Mail" , await tryGet.Result.GetValueAsync<string>("Mail").ConfigureAwait(false) },
                                {"Description" , await tryGet.Result.GetValueAsync<string>("Description").ConfigureAwait(false) }
                            };
                            return myObject;
                        }
                        else
                        {
                            return null;
                        }
                    }
                )
            );
        }
    }
}
