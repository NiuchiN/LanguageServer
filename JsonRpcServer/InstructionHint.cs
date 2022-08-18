using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace JsonRpcServer
{
    public class SignatureData
    {
        public string Signature;
        public string Description;
    }

    public class InstructionHint
    {
        // Property
        public bool IsSignatureDataEnable { get; private set; }

        // Private Member
        private string _InputFilePath;    //Signature定義が書かれたファイルへの絶対パス
        private Dictionary<string, SignatureData> _dictInstHints = new Dictionary<string, SignatureData>();
        private InstructionHashList _instHashList;

        // Constructor
        public InstructionHint(string relativePath, InstructionHashList hashList)
        {
            string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _InputFilePath = Path.Combine(exeDir, relativePath);
            _instHashList = hashList;
        }

        // Private Method
        private void readSignatureData()
        {
            using (StreamReader reader = new StreamReader(_InputFilePath))
            {
                while (0 <= reader.Peek()) {
                    SignatureData sigData = new SignatureData();
                    string textLine = reader.ReadLine().Replace(@"\n", "\n\n").Replace("<", @"\<").Replace(">", @"\>");
                    
                    // InputFile には、 InstructionName@Signature;Description　の形式で格納されているので分離する。
                    string[] strSigDesc = textLine.Split(';');
                    sigData.Description = strSigDesc.Last();

                    string[] strNameSig = strSigDesc.First().Split('@');
                    string instName = strNameSig.First();
                    sigData.Signature = strNameSig.Last();

                    _instHashList.SetInstructionToHash(instName);
                    _dictInstHints.Add(strNameSig.First(), sigData);
                }
            }
        }

        // Public Method
        public void MakeSignatureData()
        {
            try {
                readSignatureData();
                IsSignatureDataEnable = true;
            } catch {
                IsSignatureDataEnable = false;
                Console.Error.WriteLine("make signature data failed.");
            }

        }
        
        public string GetSignature(string instName)
        {
            if (_instHashList.IsExistInstruction(instName)) {
                return _dictInstHints[instName].Signature;
            } else {
                return null;
            }
        }

        public string GetDescription(string instName)
        {
            if (_instHashList.IsExistInstruction(instName)) {
                return _dictInstHints[instName].Description;
            } else {
                return null;
            }
        }
    }

    public class InstructionHashList
    {
        private HashSet<string> _instructionHash;

        public InstructionHashList()
        {
            _instructionHash = new HashSet<string>();
        }

        public bool IsExistInstruction(string instName)
        {
            return _instructionHash.Contains(instName);
        }

        public void SetInstructionToHash(string instName)
        {
            _instructionHash.Add(instName);
        }

        public List<string> GetAllList()
        {
            var retList = new List<string>();

            foreach (var item in _instructionHash) {
                retList.Add(item);
            }
            return retList;
        }
    }
}
