Copy the Dynamics.CRM.DataProcessing.Extension.dll, Microsoft.CRM.Sdk.Proxy.dll, and the Microsoft.XRM.Sdk.dll
files into 
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies

Edit the file:
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\RSReportDesigner.config
and under the Data node add the line:
<Extension Name="Dynamics 365 Data Extension" Type="Dynamics.CRM.DataProcessing.Extension.Dynamics_CRM_Connection,Dynamics.CRM.DataProcessing.Extension"/>


Edit the file:
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

