using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utility;

namespace ExtendedApplications
{
	public class ActAuthenticateResponseClass
	{
		public async Task ActAuthenticateResponse(NetworkStream stream, XmlDocument xmlDoc, string url)
		{
			string xmlFilePath = Constants.TESTDATA_PATH + "/ActAuthenticateRespose_Temp.xml";
			await Helper.Send200_ReadXmlFromFileAsync(stream, xmlFilePath);
		}
	}
}
