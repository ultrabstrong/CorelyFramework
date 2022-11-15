using System.ServiceModel;
using System.ServiceModel.Web;

namespace Corely.Services
{

    [ServiceContract]
    public interface IServiceConfigBase
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "UpdatePW?update={update}&oldpw={oldpw}&newpw={newpw}&confirmpw={confirmpw}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResponseBase UpdatePW(string update, string oldpw, string newpw, string confirmpw);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetVersion();

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResponseBase ReloadServiceConfig();
    }
}
