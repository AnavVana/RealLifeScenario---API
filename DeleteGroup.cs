using System.Threading.Tasks;
using System.Linq;
using System.Text;
using VI.Base;
using VI.DB;
using VI.DB.Entities;
using QBM.CompositionApi.ApiManager;
using QBM.CompositionApi.Definition;
using QBM.CompositionApi.Crud;
using System;
using System.Xml.Linq;

namespace QBM.CompositionApi
{
    public class DeleteObject : IApiProviderFor<QER.CompositionApi.Portal.PortalApiProject>, IApiProvider
    {
        public void Build(IApiBuilder builder)
        {
            builder.AddMethod(Method.Define("reallife/deletegroup/{UID_AADGroup}")
                .WithParameter("UID_AADGroup", typeof(string), isInQuery: false)
                .Handle<ReturnedClass>("DELETE", async (qr, ct) =>
                {
                    var strUID_AADGroup = qr.Parameters.Get<string>("UID_AADGroup");
                    var query1 = Query.From("AADGroup")
                        .Select("*")
                        .Where(string.Format("UID_AADGroup='{0}'", strUID_AADGroup));

                    var tryget1 = await qr.Session.Source()
                                        .TryGetAsync(query1, EntityLoadType.DelayedLogic, ct)
                                        .ConfigureAwait(false);

                    if (tryget1.Success)
                    {
                        // Extract uid before deletion
                        string uidToReturn = await tryget1.Result.GetValueAsync<string>("UID_AADGroup").ConfigureAwait(false);
                        using (var u = qr.Session.StartUnitOfWork())
                        {
                            var objecttodelete = tryget1.Result;

                            objecttodelete.MarkForDeletion();

                            await u.PutAsync(objecttodelete, ct).ConfigureAwait(false);

                            await u.CommitAsync(ct).ConfigureAwait(false);
                        }
                        return new ReturnedClass { strUID_AADGroup = uidToReturn };
                    }
                    else
                    {
                        return await ReturnedClass.Error(
                            $"No assignment was found with strUID_AADGroup '{strUID_AADGroup}'.", 681
                        ).ConfigureAwait(false);
                    }

                    return await ReturnedClass.fromEntity(tryget1.Result, qr.Session).ConfigureAwait(false);
                }));
        }

        public class ReturnedClass
        {
            public string strUID_AADGroup { get; set; }

            public string errormessage { get; set; }

            public static async Task<ReturnedClass> fromEntity(IEntity entity, ISession session)
            {
                var g = new ReturnedClass
                {
                    strUID_AADGroup = await entity.GetValueAsync<string>("strUID_AADGroup").ConfigureAwait(false)
                };

                return g;
            }

            public static async Task<ReturnedClass> ReturnObject(string data)
            {
                var x = new ReturnedClass
                {
                    errormessage = data
                };
                return x;
            }

            public static async Task<ReturnedClass> Error(string mess, int errorNumber)
            {
                // Throw an HTTP exception with the provided error number and message
                throw new System.Web.HttpException(errorNumber, mess);
            }

        }
    }
}
