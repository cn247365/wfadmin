using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;

namespace VHSSyncSAPToWorkflow
{
    public class MyBackendConfig : IDestinationConfiguration
    {
        public RfcConfigParameters GetParameters(String destinationName)
        {

            if ("PRD_000".Equals(destinationName))
            {

                RfcConfigParameters parms = new RfcConfigParameters();

                parms.Add(RfcConfigParameters.AppServerHost, "172.21.51.1");   //SAP主机IP  PP2 :172.21.51.240    HP1: 172.21.51.1    HQ1: 172.21.51.2   open-->cmd--->ping SAPPP2/SAPHP1/SAPHQ1--->get the IP

                parms.Add(RfcConfigParameters.SystemNumber, "20"); //SAP实例 // HP1:20    PP2:20    HQ1:10

                parms.Add(RfcConfigParameters.User, "CPIC_WF_VOIC"); //用户名

                parms.Add(RfcConfigParameters.Password, "wfvoic09"); //密码

                parms.Add(RfcConfigParameters.Client, "200");  //Client

                parms.Add(RfcConfigParameters.Language, "en");  //登陆语言

                parms.Add(RfcConfigParameters.PoolSize, "5");

                parms.Add(RfcConfigParameters.MaxPoolSize, "10");

                parms.Add(RfcConfigParameters.IdleTimeout, "60");

                return parms;

            }

            else return null;

        }

        public bool ChangeEventsSupported()
        {

            return false;

        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

    }
}
