using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Messages
{
    public class MessagesHelper
    {
        public static byte[] GetResponseDataObjects(byte[] response)
        {
            byte len = response[2];
            byte[] responseWithoutCheckSum = GetResponseWithoutCheckSum(response, len, response.Count());

            byte[] responseDataObjects = new byte[len - 13];
            Buffer.BlockCopy(responseWithoutCheckSum, 13, responseDataObjects, 0, (len - 13));

            return responseDataObjects;
        }

        private static byte[] GetResponseWithoutCheckSum(byte[] response, byte len, int totalLen)
        {
            byte[] responseWithoutCheckSum = new byte[len + 3];

            Buffer.BlockCopy(response, 0, responseWithoutCheckSum, 0, 8); //izbaci sve checksum-e
            for (int i = 10, j = 8; i <= totalLen; i += 18, j += 16)
            {
                if ((totalLen - i) >= 18)
                    Buffer.BlockCopy(response, i, responseWithoutCheckSum, j, 16);
                else
                    Buffer.BlockCopy(response, i, responseWithoutCheckSum, j, (totalLen - i - 2));
            }

            return responseWithoutCheckSum;
        }
    }
}
