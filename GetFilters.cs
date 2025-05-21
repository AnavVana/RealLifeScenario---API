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

namespace QBM.CompositionApi
{
    public class GetFilters : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject> // This is a test comment for BRANCH 1
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/getfilters")
                .HandleGet(async (qr, ct) =>
                {
                    var descriptionvalue = "THE_GROUP_description";

                    var query = Query.From("AADGroup")
                    .Select("*")
                    .Where($"Description = '{descriptionvalue}'");


                    var results = await qr.Session.Source()
                    .GetCollectionAsync((Query)query, (EntityCollectionLoadType)EntityLoadType.DelayedLogic)
                    .ConfigureAwait(false);

                    var groupList = new List<Dictionary<string, string>>();

                    foreach (var entity in results)
                    {
                        var group = new Dictionary<string, string>
                    {
                        { "DisplayName", await entity.GetValueAsync<string>("DisplayName").ConfigureAwait(false) },
                        { "UID_AADOrganization", await entity.GetValueAsync<string>("UID_AADOrganization").ConfigureAwait(false) },
                        { "MailNickName", await entity.GetValueAsync<string>("MailNickName").ConfigureAwait(false) },
                        { "Mail", await entity.GetValueAsync<string>("Mail").ConfigureAwait(false) },
                        { "Description", await entity.GetValueAsync<string>("Description").ConfigureAwait(false) }
                    };

                        groupList.Add(group);
                    }

                    return groupList;
                })
        );
        }
    }
}
