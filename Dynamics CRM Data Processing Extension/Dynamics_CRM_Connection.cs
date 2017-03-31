using System;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
namespace Dynamics.CRM.DataProcessing.Extension
{

    public class Dynamics_CRM_Connection : IDbConnection, IDbConnectionExtension, IExtension, IDisposable
    {
        private string username;
        private string password;

        private OrganizationServiceProxy service { get; set; }

        public string Impersonate
        {
            set
            {
                value = service.CallerId.ToString();
            }
        }

        public bool IntegratedSecurity { get; set; }

       
        public string ConnectionString { get; set; }
      

        public int ConnectionTimeout { get;  }


        string IExtension.LocalizedName { get; }

        public string UserName
        {
            set
            {
                username = value;
            }
        }

        public string Password
        {
            set
            {
                password = value;
            }
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return service.Execute(request);
        }

        public IDbTransaction BeginTransaction()
        {
            return new Dynamics_CRM_Transaction();
        }

        void IDbConnection.Close()
        {
           // service = null;
        }

        public IDbCommand CreateCommand()
        {
            return new Dynamics_CRM_Command(this);           
        }

        public void Dispose()
        {
            if (service != null)
            {
                service.Dispose();
            }
        }

        public void Open()
        {
            CreateService();
        }

        void IExtension.SetConfiguration(string configuration)
        {

        }


        public Dynamics_CRM_Connection()
        {

        }
        public Dynamics_CRM_Connection(string url)
        {
            ConnectionString = url;
        }

        protected void CreateService()
        {

            try
            {
                //   Uri org = new Uri(Url + @"/XRMServices/2011/Organization.svc");
                string url = ((IDbConnection)this).ConnectionString;
                if (!String.IsNullOrEmpty(url))
                {
                    if (url.Substring(url.Length - 1) != "/")
                    {
                        url += "/";
                    }
                    Uri org = new Uri(url + @"XRMServices/2011/Organization.svc");
                    IServiceManagement<IOrganizationService> OrganizationServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(org);
                    AuthenticationProviderType OrgAuthType = OrganizationServiceManagement.AuthenticationType;
                    AuthenticationCredentials authCredentials = new AuthenticationCredentials();
                    authCredentials.ClientCredentials.UserName.UserName = username;
                    authCredentials.ClientCredentials.UserName.Password = password;
                    AuthenticationCredentials tokenCredentials = OrganizationServiceManagement.Authenticate(authCredentials);

                    SecurityTokenResponse responseToken = tokenCredentials.SecurityTokenResponse;

                    service = new OrganizationServiceProxy(OrganizationServiceManagement, responseToken);
                    service.EnableProxyTypes();
                }
                else
                {
                    throw new Exception("Connection string must not be empty");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
