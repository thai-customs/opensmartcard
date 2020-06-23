using System;
using System.Runtime.InteropServices;
using System.Text;
namespace OpenSmartCard
{
    public class OPE
    {
        [ComVisible(false)]
        private static int GetHexVal(char hex) {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        [ComVisible(false)]
        private static byte[] Str2Bin(string hex) {
            if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");
            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i) {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }
            return arr;
        }
        [DllImport("scapi_ope.dll")]
        public static extern short ListReader(StringBuilder List_Reader, ref int status);
        [ComVisible(true)]
        public string ListReader()
        {
            try
            {
                int status = 0;
                StringBuilder str = new StringBuilder("", 1000);
                int rc = ListReader(str, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&readers={Uri.EscapeUriString(str.ToString())}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short OpenReader(string name, ref int status);
        [ComVisible(true)]
        public string OpenReader(string readerName)
        {
            try
            {
                int status = 0;
                int rc = OpenReader(readerName, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetCardStatus(StringBuilder atr, ref int atrLen, ref int timeOut, ref int cardType, ref int status);
        // GetCardStatus Lib "scapi_ope.dll" (ByVal atr As String, ByRef atrLen As Integer, ByVal timeOut As Integer, ByRef cardType As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetCardStatus()
        {
            StringBuilder atr = new StringBuilder("", 100);
            int status = 0;
            int atrLen = 0;
            int timeout = 0;
            int cardType = 0;
            try
            {
                int rc = GetCardStatus(atr, ref atrLen, ref timeout, ref cardType, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&atr={Uri.EscapeUriString(atr.ToString())}&atrLen={atrLen}&timeout={timeout}&cardType={cardType}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        // not test
        [DllImport("scapi_ope.dll")]
        public static extern short SelectApplet(ref byte aid, int aidSize, ref int status);
        // SelectApplet Lib "scapi_ope.dll" (ByRef aid As Byte, ByVal aid_size As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string SelectApplet(string hex) {
            try {
                int status = 0;
                byte[] aid = Str2Bin(hex);
                int rc = SelectApplet(ref aid[0], aid.Length, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short ReadData(int blockId, int offset, int dataSize, StringBuilder dataBuf, ref int status);
        // ReadData Lib "scapi_ope.dll" (ByVal block_id As Integer, ByVal offset As Integer, ByVal data_size As Integer, ByVal data_buf As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string ReadData() {
            try {
                int status = 0;
                const int blockId = 0;
                const int offset = 4;
                const int dataSize = 13;
                StringBuilder dataBuf = new StringBuilder("", 15);
                int rc = ReadData(blockId, offset, dataSize, dataBuf, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&data={dataBuf.ToString()}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetCardInfo(StringBuilder cardSN, StringBuilder chip, StringBuilder os, StringBuilder prefix, StringBuilder person, ref int status);
        // GetCardInfo Lib "scapi_ope.dll" (ByVal card_sn As String, ByVal chip As String, ByVal os As String, ByVal pre_perso As String, ByVal perso As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetCardInfo() {
            try {
                int status = 0;
                StringBuilder cardSN = new StringBuilder("", 16);
                StringBuilder chip = new StringBuilder("", 20);
                StringBuilder os = new StringBuilder("", 20);
                StringBuilder prefix = new StringBuilder("", 20);
                StringBuilder person = new StringBuilder("", 20);
                int rc = GetCardInfo(cardSN, chip, os, prefix, person, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&cardSN={cardSN.ToString()}&chip={chip.ToString()}&os={os.ToString()}&prefix={prefix.ToString()}&person={person.ToString()}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetInfoADM(StringBuilder version, ref int state, ref int authorize, StringBuilder laserNumber, ref int status);
        // GetInfoADM Lib "scapi_ope.dll" (ByVal version As String, ByRef status As Integer, ByRef authorize As Integer, ByVal laser_number As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetInfoADM() {
            try {
                int status = 0;
                StringBuilder version = new StringBuilder("", 5);
                int state = 0;
                int authorize = 0;
                StringBuilder laserNumber = new StringBuilder("", 33);
                int rc = GetInfoADM(version, ref state, ref authorize, laserNumber, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&version={version.ToString()}&state={state}&authorize={authorize}&laserNumber={laserNumber}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short VerifyPIN(int pinId, int shareData, ref int remain, ref int status);
        // VerifyPIN Lib "scapi_ope.dll" (ByVal pin_id As Integer, ByVal share_data As Integer, ByRef try_remain As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string VerifyPIN() {
            try {
                int status = 0;
                int remain = 0;
                int rc = VerifyPIN(1, 0, ref remain, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&remain={remain}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetMatchStatus(int type, int mode, string random, int randomSize, StringBuilder crypto, ref int cryptoSize, ref int matchStatus, ref int status);
        // GetMatchStatus Lib "scapi_ope.dll" (ByVal req_type As Integer, ByVal req_mode As Integer, ByVal in_buf As String, ByVal in_size As Integer, ByVal out_buf As String, ByRef out_size As Integer, ByRef match_stt As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetMatchStatus(string random) {
            try {
                int status = 0;
                StringBuilder crypto = new StringBuilder("", 64);
                int cryptoSize = 0;
                int matchStatus = 0;
                int rc = GetMatchStatus(1, 0, random, random.Length, crypto, ref cryptoSize, ref matchStatus, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&crypto={crypto.ToString()}&matchStatus={matchStatus}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short EnvelopeGMSx(int keyId, string crypto, int cryptoLen, StringBuilder request, ref int requestLen, ref int status);
        // EnvelopeGMSx Lib "scapi_ope.dll" (ByVal key_id As Integer, ByVal cryptogram As String, ByVal cryptogram_len As Integer, ByVal request As String, ByRef request_len As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string EnvelopeGMSx(int keyId, string crypto) {
            try {
                int status = 0;
                int requestLen = 255;
                StringBuilder request = new StringBuilder("", 255);
                int rc = EnvelopeGMSx(keyId, crypto, crypto.Length, request, ref requestLen, ref status);
                if (rc != 0) throw new Exception($"rc={rc}&status={status}");
                return $"status={status}&envelope={request.ToString()}";
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [ComVisible(true)]
        public void TestError(string msg) {
            throw new Exception(msg.ToString());
        }
        [ComVisible(false)]
        public string GetVersion() {
            return "Tinnakrit";
        }
    }
}
