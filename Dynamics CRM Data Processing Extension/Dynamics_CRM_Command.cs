using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Xml.Linq;
namespace Dynamics.CRM.DataProcessing.Extension
{
    public class Dynamics_CRM_Command : IDbCommand, IDbCommandAnalysis, IDisposable
    {
    //    private Dynamics_CRM_Data_Parameter_Collection pCollection;

        public Dynamics_CRM_Connection Connection { get; set; }
       
        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters
        {
            get; private set;
        }

        public IDbTransaction Transaction
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }



        public Dynamics_CRM_Command(Dynamics_CRM_Connection conn)
        {
            Parameters = new Dynamics_CRM_Data_Parameter_Collection();
            Connection = conn;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDataParameter CreateParameter()
        {
            return new Dynamics_CRM_DataParameter();
        }

        public Dynamics_CRM_DataReader ExecuteReader()
        {
            foreach (IDataParameter parameter in Parameters)
            {
                CommandText = CommandText.Replace(parameter.ParameterName, parameter.Value.ToString());
            }
            return new Dynamics_CRM_DataReader(CommandText, Connection);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            foreach(IDataParameter parameter in Parameters)
            {
                CommandText = CommandText.Replace(parameter.ParameterName, parameter.Value.ToString());
            }
            return new Dynamics_CRM_DataReader(CommandText, Connection);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Dynamics_CRM_Command() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public IDataParameterCollection GetParameters()
        {
            Dynamics_CRM_Data_Parameter_Collection pCollection = new Dynamics_CRM_Data_Parameter_Collection();
            try
            {
                XDocument doc = XDocument.Parse(CommandText);
                List<XElement> conditionElements = doc.Descendants("condition").ToList();
                foreach (XElement element in conditionElements)
                {
                    XAttribute attribute = element.Attribute("value");
                    if (attribute != null && attribute.Value.Contains("@"))
                    {
                        Dynamics_CRM_MultiValueParameter parameter = new Dynamics_CRM_MultiValueParameter();
                        parameter.ParameterName = attribute.Value;
                        pCollection.Add(parameter);
                    }
                }
            }
            catch
            {

            }
            return pCollection;
        }

        //     IDataParameterCollection IDbCommandAnalysis.GetParameters()
        //     {
        //         return pCollection;
        //     }
        #endregion
    }
}
