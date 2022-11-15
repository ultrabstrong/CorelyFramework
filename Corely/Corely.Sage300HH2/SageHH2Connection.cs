using Corely.Connections;
using Corely.Connections.Proxies;
using Corely.Security;
using Newtonsoft.Json;
using Corely.Sage300HH2.Connection;
using Corely.Sage300HH2.Core;
using Corely.Sage300HH2.Core.Document;
using Corely.Sage300HH2.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml.Serialization;
using rm = Corely.Sage300HH2.Resources.Strings;
using Corely.Sage300HH2.Core.Generic;
using Corely.Security.Authentication;

namespace Corely.Sage300HH2
{
    [Serializable]
    public class SageHH2Connection
    {
        #region Constructor

        /// <summary>
        /// Construct with secret object initialized
        /// </summary>
        public SageHH2Connection()
        {
            AuthenticationSecrets = new AESValues();
            AuthSecretsKey = AESEncryption.CreateRandomBase64Key();
        }

        /// <summary>
        /// Contstruct with connection information set
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="apikey"></param>
        /// <param name="apisecret"></param>
        public SageHH2Connection(string baseAddress, string username, string password, string apikey, string apisecret) : this()
        {
            BaseAddress = baseAddress;
            Username = username;
            AuthenticationSecrets[Password] = password;
            AuthenticationSecrets[ApiKey] = apikey;
            AuthenticationSecrets[ApiSecret] = apisecret;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Base address of web service
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Username for connecting to web service
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Encrypted authentication values
        /// </summary>
        public AESValues AuthenticationSecrets { get; set; }

        /// <summary>
        /// Key for authentication secrets
        /// </summary>
        public string AuthSecretsKey
        {
            get { return _authSecretsKey; }
            set
            {
                _authSecretsKey = value;
                AuthenticationSecrets?.SetEncryptionKey(_authSecretsKey);
            }
        }
        private string _authSecretsKey;

        /// <summary>
        /// Response object return from authentication request
        /// </summary>
        [XmlIgnore]
        public AuthenticateSessionResponse AuthenticateSessionResponse { get; internal set; }

        /// <summary>
        /// Auth token returned from valid authentication
        /// </summary>
        [XmlIgnore]
        public AuthenticationToken AuthenticationToken { get; internal set; }

        /// <summary>
        /// Http proxy with base address for sage
        /// </summary>
        [XmlIgnore]
        public HttpProxy Proxy { get; internal set; }

        /// <summary>
        /// Strings for accessing authentication secrets
        /// </summary>
        private readonly string Password = "Password";
        private readonly string ApiKey = "ApiKey";
        private readonly string ApiSecret = "ApiSecret";

        /// <summary>
        /// API Endpoint links
        /// </summary>
        private static class Links
        {
            internal static readonly string Authenticate = "Api/Security/V3/Session.svc/authenticate";
            internal static readonly string GetAccounts = "GeneralLedger/Api/V1/Account.svc/accounts";
            internal static readonly string GetCategories = "JobCosting/Api/V1/JobCost.svc/jobs/categories";
            internal static readonly string GetCostCodes = "JobCosting/Api/V1/JobCost.svc/jobs/costcodes";
            internal static readonly string GetDistributions = "AccountsPayable/Api/V1/Invoice.svc/invoices/distributions";
            internal static readonly string GetJobs = "JobCosting/Api/V1/JobCost.svc/jobs";
            internal static readonly string GetStandardCategories = "JobCosting/Api/V1/JobCost.svc/categories";
            internal static readonly string GetVendors = "AccountsPayable/Api/V1/Vendor.svc/vendors";
            internal static readonly string GetFailureSync = "Synchronization/EventService.svc/events/failures";
            internal static readonly string GetOperationStatus = "Synchronization/RequestService.svc/status";
            internal static readonly string GetDocuments = "DocumentManagement/Api/V1/Document.svc/document";
            internal static readonly string GetPostedAPDocuments = "AccountsPayable/Api/V1/Invoice.svc/invoices";
            internal static readonly string GetPostedARDocuments = "AccountsReceivable/Api/V1/Invoice.svc/invoices";
            internal static readonly string GetPayments = "AccountsPayable/Api/V1/Payment.svc/payments";
            internal static readonly string WriteDocument = "DocumentManagement/Api/V1/Document.svc/document";
            internal static readonly string UpdateDocument = "DocumentManagement/Api/V1/Document.svc/document/actions/update";
            internal static readonly string ExportDocument = "DocumentManagement/Api/V1/Document.svc/document/actions/export";
        }

        #endregion

        #region Connection Methods

        /// <summary>
        /// Send authentication request
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            // Only connect if not already connected
            if (!IsConnected())
            {
                // Create proxy for web serivce
                Proxy = new HttpProxy();
                Proxy.Connect(BaseAddress);
                // Create authentication content
                var content = new
                {
                    Username,
                    Password = AuthenticationSecrets?[Password],
                    ApiKey = AuthenticationSecrets?[ApiKey],
                    ApiSecret = AuthenticationSecrets?[ApiSecret]
                };
                // Post to endpoint for authentication
                HttpResponseMessage result = Proxy.SendRequestForHttpResponse(Links.Authenticate, Corely.Connections.HttpMethod.Post, Proxy.BuildJsonContent(JsonConvert.SerializeObject(content)), null, null);
                // Read authentication json response
                string json = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                AuthenticateSessionResponse = JsonConvert.DeserializeObject<AuthenticateSessionResponse>(json);
                // Return false if connection failed
                if (AuthenticateSessionResponse.Result != AuthenticationResult.Validated)
                {
                    throw new SageHH2NotConnectedException(AuthenticateSessionResponse.Result.ToString());
                }
                // Read cookie into authentication token
                List<string> tokenheader = result.Headers.First(header => header.Key == "Set-Cookie").Value.ToList();
                HttpCookie.TryParse(tokenheader[0], out HttpCookie cookie);
                AuthenticationToken = new AuthenticationToken(cookie.Value, cookie.Expires);
            }
        }

        /// <summary>
        /// Clear the current authentication data
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            Proxy = null;
            AuthenticateSessionResponse = null;
            AuthenticationToken = null;
        }

        /// <summary>
        /// Check if auth token is valid
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (AuthenticationToken == null || AuthenticationToken.IsValid() == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if password is the same as the current password
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public bool ValidatePassword(string pw) => pw == AuthenticationSecrets[Password];

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public void SetPassword(string pw)
        {
            AuthenticationSecrets[Password] = pw;
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Wait for HH2 operation to complete
        /// </summary>
        /// <param name="operationid"></param>
        public void WaitForHH2Operation(string operationid, int errortimeoutseconds)
        {
            int secondswaited = 0;
            // Wait until export complete
            checkstatus:
            System.Threading.Thread.Sleep(1000);
            secondswaited++;
            RequestExecutionStatus status = GetOperationStatusForOperationId(operationid);
            if (string.IsNullOrWhiteSpace(status.CompletedOn))
            {
                if (secondswaited > errortimeoutseconds)
                {
                    throw new Exception(rm.timeoutError);
                }
                goto checkstatus;
            }
        }

        /// <summary>
        /// Throw exceptions for document id
        /// </summary>
        /// <param name="docid"></param>
        /// <param name="utcdate"></param>
        public void ThrowExceptionsForDocument(string docid, DateTime utcdate)
        {
            List<SyncFailure> exportfailures = GetAggregateFailuresForEntity(docid, utcdate);
            if (exportfailures.Count > 0)
            {
                string errormessage = string.Join(Environment.NewLine, exportfailures.Select(m => m.ErrorMessage));
                throw new Exception(errormessage);
            }
        }

        #endregion

        #region Get Data Methods

        /// ACCOUNT

        /// <summary>
        /// Get accounts chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Account> GetFirstAccountsChunk()
        {
            return GetPagedResult<Account>(Links.GetAccounts, false);
        }

        /// <summary>
        /// Get all accounts
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAllAccounts()
        {
            return GetAllPagedItems<Account>(Links.GetAccounts);
        }

        /// <summary>
        /// Get account from account id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account GetAccountFromAccountId(string id)
        {
            List<Account> accounts = GetAllAccounts();
            return accounts.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get account from account code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Account GetAccountFromAccountCode(string code)
        {
            List<Account> accounts = GetAllAccounts();
            return accounts.FirstOrDefault(m => m.Code == code);
        }


        /// COST CODE

        /// <summary>
        /// Get cost codes chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<CostCode> GetFirstCostCodesChunk()
        {
            return GetPagedResult<CostCode>(Links.GetCostCodes, false);
        }

        /// <summary>
        /// Get all cost codes
        /// </summary>
        /// <returns></returns>
        public List<CostCode> GetAllCostCodes()
        {
            return GetAllPagedItems<CostCode>(Links.GetCostCodes);
        }

        /// <summary>
        /// Get cost code from cost code code
        /// </summary>
        /// <returns></returns>
        public CostCode GetCostCodeFromCostCodeCode(string code)
        {
            List<CostCode> costcodes = GetAllCostCodes();
            return costcodes.FirstOrDefault(m => m.Code == code);
        }

        /// <summary>
        /// Get cost code from cost code id
        /// </summary>
        /// <returns></returns>
        public CostCode GetCostCodeFromCostCodeId(string id)
        {
            List<CostCode> costcodes = GetAllCostCodes();
            return costcodes.FirstOrDefault(m => m.Id == id);
        }
        
        /// <summary>
        /// Get all cost codes for job id
        /// </summary>
        /// <param name="jobid"></param>
        /// <returns></returns>
        public List<CostCode> GetAllCostCodesForJobId(string jobid)
        {
            HttpParameters parameters = new HttpParameters("job", jobid);
            return GetAllPagedItems<CostCode>(Links.GetCostCodes, parameters);
        }

        /// <summary>
        /// Get cost code for job id and cost code code
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public CostCode GetCostCodeForJobIdAndCostCodeCode(string jobid, string code)
        {
            return GetAllCostCodesForJobId(jobid)?.FirstOrDefault(m => m.Code == code);
        }


        /// STANDARD CATEGORY

        /// <summary>
        /// Get standard category chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<StandardCategory> GetFirstStandardCategoryChunk()
        {
            return GetPagedResult<StandardCategory>(Links.GetStandardCategories, false);
        }

        /// <summary>
        /// Get all standard categories
        /// </summary>
        /// <returns></returns>
        public List<StandardCategory> GetAllStandardCategories()
        {
            return GetAllPagedItems<StandardCategory>(Links.GetStandardCategories);
        }

        /// <summary>
        /// Get standard category for standard category code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StandardCategory GetStandardCategoryFromStandardCategoryId(string id)
        {
            List<StandardCategory> standardCategories = GetAllStandardCategories();
            return standardCategories.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get standard category for standard category code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StandardCategory GetStandardCategoryFromStandardCategoryCode(string code)
        {
            List<StandardCategory> standardCategories = GetAllStandardCategories();
            return standardCategories.FirstOrDefault(m => m.Code == code);
        }


        /// CATEGORY

        /// <summary>
        /// Get category chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Category> GetFirstCategoryChunk()
        {
            return GetPagedResult<Category>(Links.GetCategories, false);
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public List<Category> GetAllCategories()
        {
            return GetAllPagedItems<Category>(Links.GetCategories);
        }

        /// <summary>
        /// Get category for category id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Category GetCategoryForCategoryId(string id)
        {
            List<Category> categories = GetAllCategories();
            return categories.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get category for category code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Category GetCategoryForCategoryCode(string code)
        {
            List<Category> categories = GetAllCategories();
            return categories.FirstOrDefault(m => m.Code == code);
        }

        /// <summary>
        /// Get category for job
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategoriesForJobId(string jobid)
        {
            HttpParameters parameters = new HttpParameters("job", jobid);
            return GetAllPagedItems<Category>(Links.GetCategories, parameters);
        }

        /// <summary>
        /// Get category for job and cost code
        /// </summary>
        /// <param name="job"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Category> GetCategoriesForJobIdAndCostCodeId(string jobid, string costcodeid)
        {
            return GetCategoriesForJobId(jobid)?.Where(m => m.CostCodeId == costcodeid)?.ToList();
        }


        /// JOB

        /// <summary>
        /// Get jobs chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Job> GetFirstJobsChunk()
        {
            return GetPagedResult<Job>(Links.GetJobs, false);
        }

        /// <summary>
        /// Get all jobs
        /// </summary>
        /// <returns></returns>
        public List<Job> GetAllJobs()
        {
            return GetAllPagedItems<Job>(Links.GetJobs);
        }

        /// <summary>
        /// Get job for job id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Job GetJobForJobId(string id)
        {
            List<Job> jobs = GetAllJobs();
            return jobs.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get job for job code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Job GetJobForJobCode(string code)
        {
            HttpParameters parameter = new HttpParameters("q", code);
            List<Job> jobs = GetResult<List<Job>>(Links.GetJobs, parameter);
            if (jobs?.Count > 0)
            {
                return jobs[0];
            }
            return null;
        }

        /// DISTRIBUTION

        /// <summary>
        /// Get distribution chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Core.Distribution> GetFirstDistribuitonsChunk()
        {
            return GetPagedResult<Core.Distribution>(Links.GetDistributions, false);
        }

        /// <summary>
        /// Get all distributions
        /// </summary>
        /// <returns></returns>
        public List<Core.Distribution> GetAllDistributions()
        {
            return GetAllPagedItems<Core.Distribution>(Links.GetDistributions);
        }

        /// <summary>
        /// Get distribution for invoice
        /// </summary>
        /// <returns></returns>
        public List<Core.Distribution> GetDistributionsForInvoiceId(string invoiceid)
        {
            HttpParameters parameters = new HttpParameters("invoice", invoiceid);
            return GetResult<List<Core.Distribution>>(Links.GetDistributions, parameters);
        }


        /// VENDOR

        /// <summary>
        /// Get vendor chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Vendor> GetFirstVendorsChunk()
        {
            return GetPagedResult<Vendor>(Links.GetVendors, false);
        }

        /// <summary>
        /// Get all vendors
        /// </summary>
        /// <returns></returns>
        public List<Vendor> GetAllVendors()
        {
            return GetAllPagedItems<Vendor>(Links.GetVendors);
        }

        /// <summary>
        /// Get vendor for vendor id
        /// </summary>
        /// <returns></returns>
        public Vendor GetVendorForVendorId(string id)
        {
            List<Vendor> vendors = GetAllPagedItems<Vendor>(Links.GetVendors);
            return vendors.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Get vendor for vendor code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Vendor GetVendorForCode(string code)
        {
            HttpParameters parameters = new HttpParameters("qcode", code);
            List<Vendor> vendors = GetAllPagedItems<Vendor>(Links.GetVendors, parameters);
            if (vendors?.Count > 0)
            {
                return vendors[0];
            }
            return null;
        }


        /// PAYMENT

        /// <summary>
        /// Get payments chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Payment> GetFirstPaymentsChunk()
        {
            return GetPagedResult<Payment>(Links.GetPayments, false);
        }

        /// <summary>
        /// Get all payments
        /// </summary>
        /// <returns></returns>
        public List<Payment> GetAllPayments()
        {
            return GetAllPagedItems<Payment>(Links.GetPayments);
        }

        /// <summary>
        /// Get all payments
        /// </summary>
        /// <returns></returns>
        public List<Payment> GetPaymentsForType(PaymentType type)
        {
            List<Payment> payments = GetAllPagedItems<Payment>(Links.GetPayments);
            return payments?.Where(m => m.Type == type)?.ToList();
        }

        /// <summary>
        /// Get payments for vendor id
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorId(string vendorid)
        {
            HttpParameters parameters = new HttpParameters("vendor", vendorid);
            return GetAllPagedItems<Payment>(Links.GetPayments, parameters);
        }

        /// <summary>
        /// Get payments for vendor id and type
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorIdAndType(string vendorid, PaymentType type)
        {
            List<Payment> payments = GetPaymentsForVendorId(vendorid);
            return payments?.Where(m => m.Type == type)?.ToList();
        }

        /// <summary>
        /// Get payments for vendor id and invoice code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorIdAndInvoiceCode(string vendorid, string invoicecode)
        {
            List<Payment> payments = GetPaymentsForVendorId(vendorid);
            return payments?.Where(m => m.Reference1 == invoicecode).ToList();
        }

        /// <summary>
        /// Get payments for vendor code
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorCode(string vendorcode)
        {
            Vendor vendor = GetVendorForCode(vendorcode);
            return GetPaymentsForVendorId(vendor?.Id ?? "");
        }

        /// <summary>
        /// Get payments for vendor code
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorCodeAndType(string vendorcode, PaymentType type)
        {
            Vendor vendor = GetVendorForCode(vendorcode);
            return GetPaymentsForVendorIdAndType(vendor?.Id ?? "", type);
        }

        /// <summary>
        /// Get payments for vendor code and invoice code
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <returns></returns>
        public List<Payment> GetPaymentsForVendorCodeAndInvoiceCode(string vendorcode, string invoicecode)
        {
            Vendor vendor = GetVendorForCode(vendorcode);
            return GetPaymentsForVendorIdAndInvoiceCode(vendor?.Id ?? "", invoicecode);
        }

        /// FAILURES

        /// <summary>
        /// Get failures chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<SyncFailure> GetFirstFailuresChunk()
        {
            return GetPagedResult<SyncFailure>(Links.GetFailureSync, false);
        }

        /// <summary>
        /// Get all failures
        /// </summary>
        /// <returns></returns>
        public List<SyncFailure> GetAllFailures()
        {
            return GetAllPagedItems<SyncFailure>(Links.GetFailureSync);
        }

        /// <summary>
        /// Get all failures for entity id
        /// </summary>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public List<SyncFailure> GetFailuresForEntity(string entityid)
        {
            HttpParameters parameters = new HttpParameters("entity", entityid);
            return GetAllPagedItems<SyncFailure>(Links.GetFailureSync, parameters);
        }

        /// <summary>
        /// Return aggregate failures from all failure endpoints
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<SyncFailure> GetAggregateFailuresForEntity(string entity)
        {
            // Check for sync failure errors
            List<SyncFailure> failures = GetFailuresForEntity(entity);
            // Check for last action error
            DenormalizedDocument Document = GetDocumentForId(entity);
            if (!string.IsNullOrWhiteSpace(Document.LastAction?.ErrorMessage))
            {
                // Create last action sync failure
                SyncFailure lastActionFail = new SyncFailure()
                {
                    CreatedOnUtc = Document.LastAction.ActionDateTimeAgnostic,
                    EntityId = entity,
                    ErrorMessage = $"Last Action : {Document.LastAction.ErrorMessage}",
                    Id = Guid.Empty.ToString(),
                    Version = -1,
                    TypeId = "fe05a851-f722-4869-a6ae-b4bc61412d10"
                };
                // Create failures list if not initialized
                if(failures == null) { failures = new List<SyncFailure>(); }
                // Add last action failure
                failures.Add(lastActionFail);
            }
            return failures;
        }

        /// <summary>
        /// Return aggregate failures after utc date from all failure endpoints
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<SyncFailure> GetAggregateFailuresForEntity(string entity, DateTime utcdate)
        {
            // Get all sync failures for entity
            List<SyncFailure> failures = GetAggregateFailuresForEntity(entity);
            // Filter sync failures for any failures after utc date
            failures = failures.Where(m => m.CreatedOnUtc >= utcdate).ToList();
            return failures;
        }


        /// OPERATIONS

        /// <summary>
        /// Get operation status for operation id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestExecutionStatus GetOperationStatusForOperationId(string id)
        {
            string responsexml = PostForStringContent($"{Links.GetOperationStatus}/{id}");
            return Corely.Data.Serialization.XmlSerializer.DeSerialize<RequestExecutionStatus>(responsexml);
        }


        /// HH2 DOCUMENTS

        /// <summary>
        /// Get documents chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<DenormalizedDocument> GetFirstDocumentsChunk()
        {
            return GetPagedResult<DenormalizedDocument>(Links.GetDocuments, false);
        }

        /// <summary>
        /// Get all posted AP documents
        /// </summary>
        /// <returns></returns>
        public List<DenormalizedDocument> GetAllDocuments()
        {
            return GetAllPagedItems<DenormalizedDocument>(Links.GetDocuments);
        }

        /// <summary>
        /// Get posted ap document for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DenormalizedDocument GetDocumentForId(string doc_guid)
        {
            HttpParameters parameters = new HttpParameters("id", doc_guid);
            return GetResult<DenormalizedDocument>(Links.GetDocuments, parameters);
        }


        /// POSTED AP INVOICES

        /// <summary>
        /// Get invoice chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Invoice> GetFirstAPInvoicesChunk()
        {
            return GetPagedResult<Invoice>(Links.GetPostedAPDocuments, false);
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns></returns>
        public List<Invoice> GetAllAPInvoices()
        {
            return GetAllPagedItems<Invoice>(Links.GetPostedAPDocuments);
        }
        
        /// <summary>
        /// Get invoice for vendor
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public List<Invoice> GetAPInvoicesForVendorId(string vendorid)
        {
            HttpParameters parameter = new HttpParameters("vendor", vendorid);
            return GetResult<List<Invoice>>(Links.GetPostedAPDocuments, parameter);
        }

        /// <summary>
        /// Get invoice for invoice guid
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public Invoice GetAPInvoiceForDocid(string docid)
        {
            HttpParameters parameter = new HttpParameters("document", docid);
            List<Invoice> invoices = GetResult<List<Invoice>>(Links.GetPostedAPDocuments, parameter);
            if (invoices?.Count > 0)
            {
                return invoices[0];
            }
            return null;
        }


        /// POSTED AR INVOICES

        /// <summary>
        /// Get invoice chunk
        /// </summary>
        /// <returns></returns>
        public HH2PagedResult<Invoice> GetFirstARInvoicesChunk()
        {
            return GetPagedResult<Invoice>(Links.GetPostedARDocuments, false);
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns></returns>
        public List<Invoice> GetAllARInvoices()
        {
            return GetAllPagedItems<Invoice>(Links.GetPostedARDocuments);
        }

        /// <summary>
        /// Get invoices for customer number
        /// </summary>
        /// <param name="customerid"></param>
        /// <returns></returns>
        public List<Invoice> GetARInvoiceForCustomerId(string customerid)
        {
            HttpParameters parameter = new HttpParameters("customer", customerid);
            return GetAllPagedItems<Invoice>(Links.GetPostedARDocuments, parameter);
        }

        #endregion

        #region Post Document Methods

        /// <summary>
        /// Post AP or AR invoice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="pagedResult"></param>
        /// <returns></returns>
        public string WriteDocument(CreateDocumentRequest request)
        {
            return PostJsonForStringContent(Links.WriteDocument, request);
        }

        /// <summary>
        /// Update AP or AR invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string UpdateDocument(UpdateDocumentRequest request, string docid)
        {
            HttpParameters parameters = new HttpParameters("document", docid);
            return PostJsonForStringContent(Links.UpdateDocument, request, parameters);
        }

        /// <summary>
        /// Post to export document endpoint
        /// </summary>
        /// <param name="documentid"></param>
        /// <returns></returns>
        public string ExportDocument(string documentid)
        {
            HttpParameters parameter = new HttpParameters("document", documentid);
            string responsexml = PostForStringContent(Links.ExportDocument, parameter);
            return Corely.Data.Serialization.XmlSerializer.DeSerialize<string>(responsexml);
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Get all paged items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <returns></returns>
        internal List<T> GetAllPagedItems<T>(string link, HttpParameters parameter = null) where T : IVersioned
        {
            HH2PagedResult<T> result = GetAllPagedResult<T>(link, parameter);
            return result.Items;
        }

        /// <summary>
        /// Get all paged results for link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="pagedResult"></param>
        /// <returns></returns>
        internal HH2PagedResult<T> GetAllPagedResult<T>(string link, HttpParameters parameter = null) where T : IVersioned
        {
            HH2PagedResult<T> result = GetPagedResult<T>(link, true, parameter);
            while (result.HasMore)
            {
                GetPagedResult<T>(link, true, parameter, result);
            }
            return result;
        }

        /// <summary>
        /// Get paged result for link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="pagedResult"></param>
        /// <returns></returns>
        internal HH2PagedResult<T> GetPagedResult<T>(string link, bool appendChunck, HttpParameters parameter = null, HH2PagedResult <T> pagedResult = null) where T : IVersioned
        {
            // Create result if this is a new request
            if (pagedResult == null) { pagedResult = new HH2PagedResult<T>(); }
            // Set temporary paging parameters
            if (parameter == null) { parameter = new HttpParameters(null); }
            parameter.TempParameters = new Dictionary<string, string>()
            {
                { "Version", pagedResult.Version.ToString() }
            };
            // Deserialize response
            List<T> result = GetResult<List<T>>(link, parameter);
            // Add or set result to paged result
            if (appendChunck) { pagedResult.AddItems(result); }
            else { pagedResult.SetItems(result); }
            // Add or set deserialized result
            return pagedResult;
        }

        /// <summary>
        /// Get result from parameterized search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal T GetResult<T>(string link, HttpParameters parameter = null)
        {
            // Make sure this is connected
            Connect();
            // Create cookie header
            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Set-Cookie", AuthenticationToken.Token.DecryptedValue }
            };
            string xml = Proxy.SendRequestForStringResult(link, Corely.Connections.HttpMethod.Get, null, header, parameter);
            // Deserialize response
            T result = Corely.Data.Serialization.XmlSerializer.DeSerialize<T>(xml);
            return result;
        }

        /// <summary>
        /// Post for string content
        /// </summary>
        /// <param name="link"></param>
        /// <param name=""></param>
        /// <returns></returns>
        internal string PostForStringContent(string link, HttpParameters parameter = null)
        {
            try
            {
                // Make sure this is connected
                Connect();
                // Create cookie header
                Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Set-Cookie", AuthenticationToken.Token.DecryptedValue }
            };
                // Post for result GUID
                return Proxy.SendRequestForStringResult(link, Corely.Connections.HttpMethod.Post, null, header, parameter);
            }
            catch (Exception ex)
            {
                throw StandardizePostErrors(ex);
            }
        }

        /// <summary>
        /// Post JSON for string content
        /// </summary>
        /// <param name="link"></param>
        /// <param name="requeststr"></param>
        /// <returns></returns>
        internal string PostJsonForStringContent<T>(string link, T requestobj, HttpParameters parameter = null)
        {
            try
            {
                // Make sure this is connected
                Connect();
                // Create cookie header
                Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Set-Cookie", AuthenticationToken.Token.DecryptedValue }
                };
                // Create request string
                string requeststr = JsonConvert.SerializeObject(requestobj, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-dd"
                });
                // Post for result GUID
                string result = Proxy.SendJsonRequestForStringResult(link, Corely.Connections.HttpMethod.Post, requeststr, header, parameter);
                // Remove quotes from guid
                result = result.Replace("\"", "");
                // Rreturn response
                return result;
            }
            catch(Exception ex)
            {
                throw StandardizePostErrors(ex);
            }
        }

        /// <summary>
        /// Deserialize and throw post errors
        /// </summary>
        /// <param name="ex"></param>
        internal Exception StandardizePostErrors(Exception ex)
        {
            Exception returnex = null;
            if(ex?.InnerException?.Message == null)
            {
                // Throw exception as is if no inner message found
                returnex = ex;
            }
            else
            {
                PostErrors errors = null;
                try { errors = JsonConvert.DeserializeObject<PostErrors>(ex.InnerException.Message); } catch { }
                if(errors == null)
                {
                    // Throw exception as is if inner exception is not posterrors
                    returnex = ex;
                }
                else
                {
                    // Throw with post errors
                    returnex = new Exception(errors.GetErrorsString(), ex);
                }
            }
            return returnex;
        }

        #endregion
    }
}
