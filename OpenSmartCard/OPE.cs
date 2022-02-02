using System;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
namespace OpenSmartCard
{
    public class OPE
    {
        public class ExceptionJSON {
            public int rc { get; }
            public int status { get; }
            public ExceptionJSON (int rc, int status) {
                this.rc = rc;
                this.status = status;
            }
        }
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
        public class ListReaderResult {
            public int status { get; set; }
            public string readers { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short ListReader(StringBuilder List_Reader, ref int status);
        [ComVisible(true)]
        public string ListReader()
        {
            try
            {
                ListReaderResult result = new ListReaderResult();
                int status = 0;
                StringBuilder str = new StringBuilder("", 1000);
                int rc = ListReader(str, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.readers = str.ToString();
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class OpeResult {
            public int status { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short OpenReader(string name, ref int status);
        [ComVisible(true)]
        public string OpenReader(string readerName)
        {
            try
            {
                OpeResult result = new OpeResult();
                int status = 0;
                int rc = OpenReader(readerName, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class GetCardStatusResult {
            public int status { get; set; }
            public string atr { get; set; }
            public int timeout { get; set; }
            public int cardType { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetCardStatus(StringBuilder atr, ref int atrLen, ref int timeOut, ref int cardType, ref int status);
        // GetCardStatus Lib "scapi_ope.dll" (ByVal atr As String, ByRef atrLen As Integer, ByVal timeOut As Integer, ByRef cardType As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetCardStatus()
        {
            GetCardStatusResult result = new GetCardStatusResult();
            StringBuilder atr = new StringBuilder("", 100);
            int status = 0;
            int atrLen = 0;
            int timeout = 0;
            int cardType = 0;
            try
            {
                int rc = GetCardStatus(atr, ref atrLen, ref timeout, ref cardType, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.atr = atr.ToString().Substring(0, atrLen);
                result.timeout = timeout;
                result.cardType = cardType;
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        // not test
        [DllImport("scapi_ope.dll")]
        public static extern short SelectApplet(ref byte aid, int aidSize, ref int status);
        // SelectApplet Lib "scapi_ope.dll" (ByRef aid As Byte, ByVal aid_size As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string SelectApplet(string hex) {
            try {
                OpeResult result = new OpeResult();
                int status = 0;
                byte[] aid = Str2Bin(hex);
                int rc = SelectApplet(ref aid[0], aid.Length, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class ReadDataResult {
            public int status { get; set; }
            public string data { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short ReadData(int blockId, int offset, int dataSize, StringBuilder dataBuf, ref int status);
        // ReadData Lib "scapi_ope.dll" (ByVal block_id As Integer, ByVal offset As Integer, ByVal data_size As Integer, ByVal data_buf As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string ReadData() {
            try {
                ReadDataResult result = new ReadDataResult();
                int status = 0;
                const int blockId = 0;
                const int offset = 4;
                const int dataSize = 13;
                StringBuilder dataBuf = new StringBuilder("", 15);
                int rc = ReadData(blockId, offset, dataSize, dataBuf, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.data = dataBuf.ToString().Substring(0, dataSize);
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class GetCardInfoResult {
            public int status { get; set; }
            public string cardSN { get; set; }
            public string chip { get; set; }
            public string os { get; set; }
            public string prefix { get; set; }
            public string person { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetCardInfo(StringBuilder cardSN, StringBuilder chip, StringBuilder os, StringBuilder prefix, StringBuilder person, ref int status);
        // GetCardInfo Lib "scapi_ope.dll" (ByVal card_sn As String, ByVal chip As String, ByVal os As String, ByVal pre_perso As String, ByVal perso As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetCardInfo() {
            try {
                GetCardInfoResult result = new GetCardInfoResult();
                int status = 0;
                StringBuilder cardSN = new StringBuilder("", 20);
                StringBuilder chip = new StringBuilder("", 20);
                StringBuilder os = new StringBuilder("", 20);
                StringBuilder prefix = new StringBuilder("", 20);
                StringBuilder person = new StringBuilder("", 20);
                int rc = GetCardInfo(cardSN, chip, os, prefix, person, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.cardSN = cardSN.ToString().Substring(0, 16);
                result.chip = chip.ToString().Substring(0, 8);
                result.os = os.ToString().Substring(0, 12);
                result.prefix = prefix.ToString().Substring(0, 16);
                result.person = person.ToString().Substring(0, 16);
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class GetInfoADMResult {
            public int status { get; set; }
            public string version { get; set; }
            public int state { get; set; }
            public int authorize { get; set; }
            public string laserNumber { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetInfoADM(StringBuilder version, ref int state, ref int authorize, StringBuilder laserNumber, ref int status);
        // GetInfoADM Lib "scapi_ope.dll" (ByVal version As String, ByRef status As Integer, ByRef authorize As Integer, ByVal laser_number As String, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetInfoADM() {
            try {
                GetInfoADMResult result = new GetInfoADMResult();
                int status = 0;
                StringBuilder version = new StringBuilder("", 5);
                int state = 0;
                int authorize = 0;
                StringBuilder laserNumber = new StringBuilder("", 33);
                int rc = GetInfoADM(version, ref state, ref authorize, laserNumber, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.version = version.ToString();
                result.state = state;
                result.authorize = authorize;
                result.laserNumber = laserNumber.ToString();
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class VerifyPINResult {
            public int status { get; set; }
            public int remain { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short VerifyPIN(int pinId, int shareData, ref int remain, ref int status);
        // VerifyPIN Lib "scapi_ope.dll" (ByVal pin_id As Integer, ByVal share_data As Integer, ByRef try_remain As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string VerifyPIN() {
            try {
                VerifyPINResult result = new VerifyPINResult();
                int status = 0;
                int remain = 0;
                int rc = VerifyPIN(1, 0, ref remain, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.remain = remain;
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class GetMatchStatusResult {
            public int status { get; set; }
            public string crypto { get; set; }
            public int matchStatus { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short GetMatchStatus(int type, int mode, string random, int randomSize, StringBuilder crypto, ref int cryptoSize, ref int matchStatus, ref int status);
        // GetMatchStatus Lib "scapi_ope.dll" (ByVal req_type As Integer, ByVal req_mode As Integer, ByVal in_buf As String, ByVal in_size As Integer, ByVal out_buf As String, ByRef out_size As Integer, ByRef match_stt As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string GetMatchStatus(string random) {
            try {
                GetMatchStatusResult result = new GetMatchStatusResult();
                int status = 0;
                StringBuilder crypto = new StringBuilder("", 64);
                int cryptoSize = 0;
                int matchStatus = 0;
                int rc = GetMatchStatus(1, 0, random, random.Length, crypto, ref cryptoSize, ref matchStatus, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.crypto = crypto.ToString().Substring(0, cryptoSize);
                result.matchStatus = matchStatus;
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public class EnvelopeGMSxResult {
            public int status { get; set; }
            public string envelope { get; set; }
        }
        [DllImport("scapi_ope.dll")]
        public static extern short EnvelopeGMSx(int keyId, string crypto, int cryptoLen, StringBuilder request, ref int requestLen, ref int status);
        // EnvelopeGMSx Lib "scapi_ope.dll" (ByVal key_id As Integer, ByVal cryptogram As String, ByVal cryptogram_len As Integer, ByVal request As String, ByRef request_len As Integer, ByRef status As Integer) As Short
        [ComVisible(true)]
        public string EnvelopeGMSx(int keyId, string crypto) {
            try {
                EnvelopeGMSxResult result = new EnvelopeGMSxResult();
                int status = 0;
                int requestLen = 255;
                StringBuilder request = new StringBuilder("", 255);
                int rc = EnvelopeGMSx(keyId, crypto, crypto.Length, request, ref requestLen, ref status);
                if (rc != 0) throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(rc, status)));
                result.status = status;
                result.envelope = request.ToString().Substring(0, requestLen);
                return JsonConvert.SerializeObject(result);
            } catch (Exception ex) { throw new Exception(ex.Message); }
        }
        [ComVisible(true)]
        public void TestError() {
            throw new Exception(JsonConvert.SerializeObject(new ExceptionJSON(0, 0)));
        }
        public class Version {
            public string author { get; set; }
            public string version { get; set; }
            public int build { get; set; }
        }
        [ComVisible(true)]
        public string GetVersion() {
            Version version = new Version();
            version.author = "Tinnakrit";
            version.version = "7.2.2.2";
            version.build = 1;
            return JsonConvert.SerializeObject(version);
        }
    }
}
