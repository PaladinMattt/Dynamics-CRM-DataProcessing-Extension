
<!DOCTYPE html>
<html lang="en">
  <head>
  </head>
  <body>
<h1>**IMPORTANT**</h1>
<p>
Please note that although this currently works when running the queries and report from Visual Studio 2015 Report builder, 
Dynamics CRM will give you an Error if you try to upload this into CRM.  This is due to the slightly different formats of SSDT
(which is probably why Microsoft has not released a product for visual studio 2015)
You can actually get around that by editing the .rdl files xml, and mimicking that of SSDT 2012.
Since many people won't want to do this, I do NOT recommand using this for production use, merely as a learning tool.
</p>

<h2>How to Install</h2>
<p>
Copy the Dynamics.CRM.DataProcessing.Extension.dll, Microsoft.CRM.Sdk.Proxy.dll, and the Microsoft.XRM.Sdk.dll
files into 
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies
</p>


<h3>Edit the file:</h3>
<p>
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\RSReportDesigner.config
and under the Data node add the line:
<Extension Name="Dynamics 365 Data Extension" Type="Dynamics.CRM.DataProcessing.Extension.Dynamics_CRM_Connection,Dynamics.CRM.DataProcessing.Extension"/>
<p>


<h3>Edit the file:</h3>
<p>
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\RSPreviewPolicy.config
and add the following code group
 <CodeGroup
    class="UnionCodeGroup"
    version="1"
    PermissionSetName="FullTrust"
    Name="Dynamics365DataExtensionCodeGroup"
    Description="Code group for custom data processing extension">	
    <IMembershipCondition
    class="UrlMembershipCondition"
        version="1"
        Url="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\Dynamics.CRM.DataProcessing.Extension.dll" />
    </CodeGroup>
</p>

</body>
</html>