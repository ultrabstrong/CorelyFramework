using DocuWare.Platform.ServerClient;
using UsefulUtilities.DocuWareService.Core;
using UsefulUtilities.DocuWareService.Core.Responses;
using UsefulUtilities.Logging;
using UsefulUtilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using rm = UsefulUtilities.DocuWareService.Resources.ServiceResponses;
using logrm = UsefulUtilities.DocuWareService.Resources.Logs;
using UsefulUtilities.Data.Delimited;

namespace UsefulUtilities.DocuWareService
{
    public class DWService : IDWService
    {
        /// <summary>
        /// Get user info for user name
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public Core.Responses.UserInfo GetUserInfo(string apikey, string connectionid, string username)
        {
            return GenericRequestProcessor.ConnectAndProcess<Core.Responses.UserInfo>(apikey, connectionid, rm.failGetUserInfo, (response, connection) => {
                // Get users
                List<User> users = connection.Organizations[0].GetUsersFromUsersRelation().User;
                User user = users.FirstOrDefault(m => m.Name.ToLower() == username.ToLower());
                // Check if user was found
                if(user == null)
                {
                    // Return not found response
                    response.SetWithStatus($"{rm.userNotFound} {username}", HttpStatusCode.NotFound);
                }
                else
                {
                    // Set response with user info
                    user = user.GetUserFromSelfRelation();
                    response.FirstName = user.FirstName;
                    response.LastName = user.LastName;
                    response.Email = user.EMail;
                    response.UserName = user.Name;
                    response.IsOutOfOffice = user.OutOfOffice.IsOutOfOffice;
                }
            }, (nameof(username), username));
        }

        /// <summary>
        /// Get roles for user
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public MultiValue GetUserRoles(string apikey, string connectionid, string username)
        {
            return GenericRequestProcessor.ConnectAndProcess<MultiValue>(apikey, connectionid, rm.failGetUserRoles, (response, connection) => {
                // Get users
                List<User> users = connection.Organizations[0].GetUsersFromUsersRelation().User;
                User user = users.FirstOrDefault(m => m.Name.ToLower() == username.ToLower());
                // Check if user was found
                if (user == null)
                {
                    // Return not found response
                    response.SetWithStatus($"{rm.userNotFound} {username}", HttpStatusCode.NotFound);
                }
                else
                {
                    // Get roles for user
                    List<Role> userRoles = user.GetRolesFromRolesRelation().Item;
                    // Check if roles were found
                    if(userRoles.Count == 0)
                    {
                        // Return not found response
                        response.SetWithStatus($"{rm.rolesNotFoundForUser} {username}", HttpStatusCode.NotFound);
                    }
                    else
                    {
                        // Set response with role names
                        List<string> userRoleNames = userRoles.Select(m => m.Name).ToList();
                        response.Values = userRoleNames;
                        response.Message = string.Join(",", userRoleNames);
                    }
                }
            }, (nameof(username), username));
        }

        /// <summary>
        /// Get users in role
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <param name="rolename"></param>
        /// <returns></returns>
        public MultiValue GetUsersInRole(string apikey, string connectionid, string rolename)
        {
            return GenericRequestProcessor.ConnectAndProcess<MultiValue>(apikey, connectionid, rm.failGetUsersInRole, (response, connection) => {
                // Get users
                List<User> users = connection.Organizations[0].GetUsersFromUsersRelation().User;
                // Check each user's assigned roles for role name
                List<string> usersInRole = new List<string>();
                foreach(User user in users)
                {
                    List<Role> userRoles = user.GetRolesFromRolesRelation().Item;
                    if(userRoles.Any(m => m.Name.ToLower() == rolename.ToLower()))
                    {
                        // Add user to role's user list
                        usersInRole.Add(user.Name);
                    }
                }
                // Check if users were found for role
                if (usersInRole.Count == 0)
                {
                    // Return not found response
                    response.SetWithStatus($"{rm.usersNotFoundForRole} {rolename}", HttpStatusCode.NotFound);
                }
                else
                {
                    // Set response with user info
                    response.Values = usersInRole;
                    response.Message = string.Join(",", usersInRole);
                }
            }, (nameof(rolename), rolename));
        }

        /// <summary>
        /// Check if all users in role are out of office
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <param name="rolename"></param>
        /// <returns></returns>
        public ServiceResponseBase AreAllUsersInRoleOutOfOffice(string apikey, string connectionid, string rolename)
        {
            return GenericRequestProcessor.ConnectAndProcess<ServiceResponseBase>(apikey, connectionid, rm.failGetUserOOOForRole, (response, connection) => {
                // Get users
                List<User> users = connection.Organizations[0].GetUsersFromUsersRelation().User;
                if(users.Count == 0)
                {
                    // Return not found response
                    response.SetWithStatus($"{rm.usersNotFoundForRole} {rolename}", HttpStatusCode.NotFound);
                }
                else
                {
                    // Check each user's assigned roles for role name
                    List<bool> userOOOInRole = new List<bool>();
                    foreach (User user in users)
                    {
                        List<Role> userRoles = user.GetRolesFromRolesRelation().Item;
                        if (userRoles.Any(m => m.Name.ToLower() == rolename.ToLower()))
                        {
                            // Add user to role's user list
                            userOOOInRole.Add(user.OutOfOffice.IsOutOfOffice);
                        }
                    }
                    // Check if all users are out of office
                    bool allOOO = userOOOInRole.All(m => m);
                    response.Data = allOOO.ToString();
                }
            }, (nameof(rolename), rolename));
        }

        /// <summary>
        /// Query file cabinet using query settings for single column
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="connectionid"></param>
        /// <param name="cabinetid"></param>
        /// <param name="querysettings"></param>
        /// <param name="resultcolumn"></param>
        /// <returns></returns>
        public MultiValue GetListFromCabinet(string apikey, string connectionid, string cabinetid, string querysettings, string resultcolumnname)
        {
            return GenericRequestProcessor.ConnectAndProcess<MultiValue>(apikey, connectionid, rm.failGetUserOOOForRole, (response, connection) =>
            {
                // Build base query expression
                DialogExpression dex = new DialogExpression()
                {
                    Condition = new List<DialogExpressionCondition>() { },
                    Operation = DialogExpressionOperation.And,
                    Count = int.MaxValue
                };
                // Iterate query pairs and build search expressions
                DelimitedReader dr = new DelimitedReader(',', '"', ";");
                List<ReadRecordResult> queryPairs = dr.ReadAllStringRecords(querysettings);
                if (queryPairs.Any(m => m.Tokens.Count != 3)) { throw new BadRequestException(BadRequestType.InvalidParameters, rm.querySettingsInvalid); }
                foreach (ReadRecordResult queryPair in queryPairs)
                {
                    // Add query expression for comparison operator
                    switch (queryPair.Tokens[1])
                    {
                        case "<":
                            dex.Condition.Add(DialogExpressionCondition.Create(queryPair.Tokens[0], "", queryPair.Tokens[2]));
                            break;
                        case ">":
                            dex.Condition.Add(DialogExpressionCondition.Create(queryPair.Tokens[0], queryPair.Tokens[2], ""));
                            break;
                        default:
                        case "=":
                            dex.Condition.Add(DialogExpressionCondition.Create(queryPair.Tokens[0], queryPair.Tokens[2]));
                            break;                            
                    }
                }
                // Get file cabinet and dialog to query
                FileCabinet cabinet = connection.GetFileCabinet(cabinetid).GetFileCabinetFromSelfRelation();
                Dialog search = cabinet?.GetDialogInfosFromDialogsRelation().Dialog.FirstOrDefault(m => m.Type == DialogTypes.Search)?.GetDialogFromSelfRelation();
                if(search == null) { response.SetWithStatus(rm.noSearchForCabId, HttpStatusCode.NotFound); }
                else
                {
                    // Build list of return values
                    List<Document> docs = search.GetDocumentsResult(dex).Items;
                    for (int i = 0; i < docs.Count; i++)
                    {
                        docs[i] = docs[i].GetDocumentFromSelfRelation();
                        response.Values.Add(docs[i].Fields.FirstOrDefault(m => m.FieldName == resultcolumnname)?.Item?.ToString());
                    }
                }
            }, (nameof(cabinetid), cabinetid), (nameof(querysettings), querysettings), (nameof(resultcolumnname), resultcolumnname));
        }
    }
}
