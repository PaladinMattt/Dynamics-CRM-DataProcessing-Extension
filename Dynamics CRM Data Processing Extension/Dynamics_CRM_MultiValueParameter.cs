
using System.Collections.ObjectModel;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace Dynamics.CRM.DataProcessing.Extension
{
    public class Dynamics_CRM_MultiValueParameter : IDataMultiValueParameter
    {
        public string ParameterName { get; set; }
        

        public object Value { get; set; }

        public object[] Values { get; set; }

        public Dynamics_CRM_MultiValueParameter()
        {

        }
    }
}
