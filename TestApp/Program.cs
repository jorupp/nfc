using System;
using System.Net;
using System.Net.Sockets;
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
            //var comLayer = new PN532CommunicationSPI(SPI.SPI_module.SPI1, Pins.GPIO_PIN_D7);
            var comLayer = new PN532CommunicationHSU(SerialPorts.COM1);
            var nfc = new NfcPN532Reader(comLayer);
            nfc.TagDetected += nfc_TagDetected;
            nfc.TagLost += NfcOnTagLost;
            nfc.Open(NfcTagType.MifareClassic1k);

            while(true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void NfcOnTagLost(object sender, NfcTagEventArgs e)
        {
            Debug.Print("Lost : " + e.Connection.ID + ": " + e.NfcTagType);
        }

        static void nfc_TagDetected(object sender, NfcTagEventArgs e)
        {
            Debug.Print("Detected: " + e.Connection.ID + ": " + e.NfcTagType);
        }

    }
}
