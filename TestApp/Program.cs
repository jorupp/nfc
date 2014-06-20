using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using uPLibrary.Hardware.Nfc;
using uPLibrary.Nfc;

namespace TestApp
{
    public class Program
    {
        public static void Main()
        {
            // write your code here

            //var comLayer = new PN532CommunicationSPI(SPI.SPI_module.SPI1, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D6);
            var comLayer = new PN532CommunicationSPI(SPI.SPI_module.SPI1, Pins.GPIO_PIN_D7);
            //var comLayer = new PN532CommunicationHSU(SerialPorts.COM1);
            //var comLayer = new PN532CommunicationI2C(Pins.GPIO_PIN_D8);
            var nfc = new NfcPN532Reader(comLayer);
            nfc.TagDetected += (s, e) =>
                                   {
                                       Debug.Print("Detected: " + HexToString(e.Connection.ID) + ": " + e.NfcTagType);
                                       //GetData(e);
                                   };
            nfc.TagLost += (s,e) =>
                               {
                                   Debug.Print("Lost : " + HexToString(e.Connection.ID) + ": " + e.NfcTagType);
                               };
            nfc.Open(NfcTagType.MifareClassic1k);

            while(true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void GetData(NfcTagEventArgs e)
        {

            byte[] data; 
 
            switch (e.NfcTagType) 
            { 
                case NfcTagType.MifareClassic1k: 
 
                    NfcMifareTagConnection mifareConn = (NfcMifareTagConnection)e.Connection; 

                    for(byte i = 0; i< 0x10; i++)
                    {
                        var auth = new byte[] {0, 0, 0, 0, 0, 0};
                        for(long j = 0x01000000000000; j >= 0 ; j--)
                        {
                            auth[0] = (byte)((j >> 40) & 0xff);
                            auth[1] = (byte)((j >> 32) & 0xff);
                            auth[2] = (byte)((j >> 24) & 0xff);
                            auth[3] = (byte)((j >> 16) & 0xff);
                            auth[4] = (byte)((j >> 8) & 0xff);
                            auth[5] = (byte)((j >> 0) & 0xff);
                            var authRet = mifareConn.Authenticate(MifareKeyAuth.KeyA, i, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
                            if (authRet)
                            {
                                Debug.Print("A" + i + "-" + j + ": " + authRet);
                                break;
                            }

                            if(j % 64 == 0)
                                Debug.Print("A" + i + "-" + j);
                        }

                        var dataBlock = mifareConn.Read(i);
                        if(null == dataBlock)
                        {
                            Debug.Print("B" + i + ": null");
                        }
                        else
                        {
                            Debug.Print("B" + i + ": " + HexToString(dataBlock));
                        }
                    }


                    //var authRet = mifareConn.Authenticate(MifareKeyAuth.KeyA, 0x08, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
                    //Debug.Print("authRet: " + authRet);
                    //var block1 = mifareConn.Read(0x08);
                    //Debug.Print("Block1: " + HexToString(block1));
 
                    //data = new byte[16]; 
                    //for (byte i = 0; i < data.Length; i++) 
                    //    data[i] = i; 
 
                    //var retVal = mifareConn.Write(0x08, data);
                    //Debug.Print("Write: " + retVal);

                    //var dataRead = mifareConn.Read(0x08);
                    //Debug.Print("Read: " + HexToString(dataRead));

                    break; 
 
                case NfcTagType.MifareUltralight: 
 
                    NfcMifareUlTagConnection mifareUlConn = (NfcMifareUlTagConnection)e.Connection; 
 
                    for (byte i = 0; i < 16; i++) 
                    { 
                        byte[] read = mifareUlConn.Read(i); 
                        Debug.Print("Read: " + HexToString(read));
                    } 
 
                    mifareUlConn.Read(0x08); 
 
                    data = new byte[4]; 
                    for (byte i = 0; i < data.Length; i++) 
                    data[i] = i; 
 
                    mifareUlConn.Write(0x08, data); 
 
                    mifareUlConn.Read(0x08); 
                    break; 
 
                default: 
                    break; 
            } 
        }

        static private string hexChars = "0123456789ABCDEF";

        /// <summary> 
        /// Convert hex byte array in a hex string 
        /// </summary> 
        /// <param name="value">Byte array with hex values</param> 
        /// <returns>Hex string</returns> 
        static internal string HexToString(byte[] value)
        {
            StringBuilder hexString = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                hexString.Append(hexChars[(value[i] >> 4) & 0x0F]);
                hexString.Append(hexChars[value[i] & 0x0F]);
            }
            return hexString.ToString();
        } 
    }
}
