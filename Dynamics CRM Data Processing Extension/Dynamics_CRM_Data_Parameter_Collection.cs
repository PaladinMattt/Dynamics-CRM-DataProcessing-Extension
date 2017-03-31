using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Dynamics.CRM.DataProcessing.Extension
{
    
    class Dynamics_CRM_Data_Parameter_Collection : List<IDataParameter>, IDataParameterCollection 
    {
        
        public Dynamics_CRM_Data_Parameter_Collection()
        {
          
        }

        int IDataParameterCollection.Add(IDataParameter parameter)
        {
            this.Add(parameter);
            return this.Count - 1;
        }

    }
    
}
