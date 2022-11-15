using UsefulUtilities.DocuWareService.Core.Responses;
using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace UsefulUtilities.DocuWareService
{
    [ServiceContract]
    public interface IDWService
    {
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        UserInfo GetUserInfo(string apikey, string connectionid, string username);

        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        MultiValue GetUserRoles(string apikey, string connectionid, string username);

        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        MultiValue GetUsersInRole(string apikey, string connectionid, string rolename);

        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        ServiceResponseBase AreAllUsersInRoleOutOfOffice(string apikey, string connectionid, string rolename);

        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        MultiValue GetListFromCabinet(string apikey, string connectionid, string cabinetid, string querysettings, string resultcolumn);

    }
}
