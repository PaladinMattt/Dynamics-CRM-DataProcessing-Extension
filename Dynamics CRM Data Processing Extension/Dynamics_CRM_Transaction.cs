using System;
using System.Collections;

using System.Collections.ObjectModel;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;


namespace Dynamics.CRM.DataProcessing.Extension 
{
    class Dynamics_CRM_Transaction : IDbTransaction
    {
        public void Commit()
        {
            //Do nothing.
        }

        public void Dispose()
        {
            //Do nothing
        }

        public void Rollback()
        {
            //Do nothing
        }

        public Dynamics_CRM_Transaction()
        {

        }
    }
}
