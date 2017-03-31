using System;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
namespace Dynamics.CRM.DataProcessing.Extension
{
    
    class Dynamics_CRM_DataParameter : IDataParameter, IDataMultiValueParameter
    {
        public string ParameterName { get; set; }


        public object Value { get; set; }

        public object[] Values { get; set; }

        public Dynamics_CRM_DataParameter()
        {

        }

    }
    
}
